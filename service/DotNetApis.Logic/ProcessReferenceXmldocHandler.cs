using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DotNetApis.Cecil;
using DotNetApis.Common;
using DotNetApis.Nuget;
using DotNetApis.Storage;
using Microsoft.Extensions.Logging;
using Mono.Cecil;

namespace DotNetApis.Logic
{
    public sealed class ProcessReferenceXmldocHandler
    {
        private readonly ILogger _logger;
        private readonly Lazy<Task<ReferenceAssemblies>> _referenceAssemblies;
        private readonly IReferenceXmldocTable _referenceXmldocTable;
        private readonly IReferenceStorage _referenceStorage;
        private readonly IStorageBackend _storageBackend;

        private const int BatchSize = 20;
        private static readonly ReaderParameters ReaderParameters = new ReaderParameters
        {
            // We are only processing xmldoc and dnaids for accessible classes/members in each assembly, not processing anything like attributes which require assembly resolution.
            AssemblyResolver = new NullAssemblyResolver(),
        };

        public ProcessReferenceXmldocHandler(ILogger logger, Lazy<Task<ReferenceAssemblies>> referenceAssemblies, IReferenceXmldocTable referenceXmldocTable,
            IReferenceStorage referenceStorage, IStorageBackend storageBackend)
        {
            _logger = logger;
            _referenceAssemblies = referenceAssemblies;
            _referenceXmldocTable = referenceXmldocTable;
            _referenceStorage = referenceStorage;
            _storageBackend = storageBackend;
        }

        public async Task HandleAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            var referenceAssemblies = await _referenceAssemblies.Value.ConfigureAwait(false);
            stopwatch.Stop();
            _logger.LogDebug("Reference assemblies loaded in {elapsed}", stopwatch.Elapsed);
            var maxDegreeOfParallelism = _storageBackend.SupportsConcurrency ? 64 : 1;
            _logger.LogInformation("Using {threadCount} threads", maxDegreeOfParallelism);
            using (ThreadPoolTurbo.Engage(maxDegreeOfParallelism))
            {
                var processor = new ReferenceAssembliesProcessor(_logger, _referenceXmldocTable, _referenceStorage, _storageBackend);
                foreach (var referenceTarget in referenceAssemblies.ReferenceTargets)
                {
                    await processor.ProcessAsync(referenceTarget).ConfigureAwait(false);
                }
                await processor.FlushAsync().ConfigureAwait(false);
            }
        }

        private sealed class ReferenceAssembliesProcessor
        {
            private readonly ILogger _logger;
            private readonly IReferenceXmldocTable _referenceXmldocTable;
            private readonly IReferenceStorage _referenceStorage;
            private readonly ActionBlock<IBatch> _block;
            private readonly int _maxDegreeOfParallelism;

            public ReferenceAssembliesProcessor(ILogger logger, IReferenceXmldocTable referenceXmldocTable, IReferenceStorage referenceStorage, IStorageBackend storageBackend)
            {
                _logger = logger;
                _referenceXmldocTable = referenceXmldocTable;
                _referenceStorage = referenceStorage;
                _maxDegreeOfParallelism = storageBackend.SupportsConcurrency ? 64 : 1;
                _block = new ActionBlock<IBatch>(async batch =>
                {
                    try
                    {
                        await batch.ExecuteAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical(0, ex, "Unable to save to reference xmldoc table");
                        throw;
                    }
                }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = _maxDegreeOfParallelism });
            }

            public async Task ProcessAsync(ReferenceAssemblies.ReferenceTarget referenceTarget)
            {
                _logger.LogInformation("Processing reference target {referenceTarget}", referenceTarget);

                var target = referenceTarget.Target;
                using (var processor = new PlatformTargetProcessor(_logger, _block, _referenceXmldocTable, target))
                {
                    foreach (var path in referenceTarget.Paths.Where(x => x.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        // Download the dll into memory
                        var stream = await _referenceStorage.DownloadAsync(path).ConfigureAwait(false);
                        var count = 0;

                        try
                        {
                            // Load it into Cecil.
                            var assembly = AssemblyDefinition.ReadAssembly(stream, ReaderParameters);

                            // Process every member.
                            foreach (var type in assembly.Modules.SelectMany(x => x.Types).Where(x => x.IsExposed()))
                            {
                                try
                                {
                                    processor.Process(type);
                                    ++count;
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogCritical(0, ex, "Internal error");
                                    throw;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(0, ex, "Unable to load assembly {path}", path);
                        }

                        _logger.LogInformation("Processed {count} types in assembly {path}", count, path);

                        // If any processing errors yet, fail-fast.
                        if (_block.Completion.IsFaulted)
                            await _block.Completion.ConfigureAwait(false);
                    }
                }
            }

            public async Task FlushAsync()
            {
                _block.Complete();
                var blockTask = _block.Completion;
                while (await Task.WhenAny(blockTask, Task.Delay(TimeSpan.FromSeconds(3))) != blockTask)
                {
                    _logger.LogDebug("Waiting for processing to complete; remaining items: {count}", _block.InputCount);
                }
                await blockTask.ConfigureAwait(false); // propagate exceptions
            }
        }

        private sealed class PlatformTargetProcessor : IDisposable
        {
            private readonly ILogger _logger;
            private readonly ActionBlock<IBatch> _block;
            private readonly IReferenceXmldocTable _referenceXmldocTable;
            private readonly PlatformTarget _target;
            private readonly HashSet<string> _processedOverloadXmldocIds;
            private IBatch _currentBatch;

            public PlatformTargetProcessor(ILogger logger, ActionBlock<IBatch> block, IReferenceXmldocTable referenceXmldocTable, PlatformTarget target)
            {
                _logger = logger;
                _block = block;
                _referenceXmldocTable = referenceXmldocTable;
                _target = target;
                _processedOverloadXmldocIds = new HashSet<string>();
                _currentBatch = _referenceXmldocTable.CreateBatch();
            }

            public void Dispose()
            {
                if (_currentBatch.Count == 0)
                    return;
                _block.Post(_currentBatch);
            }

            public void Process(TypeDefinition type)
            {
                Add(type);

                foreach (var member in type.ExposedMembers())
                {
                    if (member is TypeDefinition nestedType)
                    {
                        Process(nestedType);
                    }
                    else
                    {
                        Add(member);
                        if (member is MethodDefinition method)
                            AddOverload(method);
                    }
                }
            }

            private void Add(IMemberDefinition member)
            {
                var dnaId = member.MemberDnaId();
                var xmldocId = member.MemberXmldocIdentifier();
                var friendlyName = member.CreateFriendlyName();

                AddToBatch(xmldocId, new ReferenceXmldocTableRecord { DnaId = dnaId, FriendlyName = friendlyName });
            }

            private void AddOverload(MethodDefinition method)
            {
                var dnaId = method.OverloadDnaId();
                var xmldocId = method.OverloadXmldocIdentifier();
                var friendlyName = method.CreateOverloadFriendlyName();

                if (_processedOverloadXmldocIds.Contains(xmldocId))
                    return;
                _processedOverloadXmldocIds.Add(xmldocId);
                AddToBatch(xmldocId, new ReferenceXmldocTableRecord { DnaId = dnaId, FriendlyName = friendlyName });
            }

            private void AddToBatch(string xmldocId, ReferenceXmldocTableRecord record)
            {
                if (_currentBatch.Count >= BatchSize)
                {
                    _block.Post(_currentBatch);
                    _currentBatch = _referenceXmldocTable.CreateBatch();
                }
                _currentBatch.Add(_referenceXmldocTable.CreateSetRecordAction(_target, xmldocId, record));
            }
        }
    }
}
