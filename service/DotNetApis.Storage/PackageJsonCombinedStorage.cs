using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Nuget;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DotNetApis.Storage
{
    public sealed class PackageJsonCombinedStorage
    {
        private readonly ILogger<PackageJsonCombinedStorage> _logger;
        private readonly IPackageJsonTable _table;
        private readonly IPackageJsonStorage _storage;

        public PackageJsonCombinedStorage(ILoggerFactory loggerFactory, IPackageJsonTable table, IPackageJsonStorage storage)
        {
            _logger = loggerFactory.CreateLogger<PackageJsonCombinedStorage>();
            _table = table;
            _storage = storage;
        }

        /// <summary>
        /// Writes a complete JSON string to storage.
        /// </summary>
        /// <param name="idver">The id and version of the package.</param>
        /// <param name="target">The target for the package.</param>
        /// <param name="docJson">The JSON documentation for the specified package id, version, and target.</param>
        public async Task WriteSuccessAsync(NugetPackageIdVersion idver, PlatformTarget target, string docJson)
        {
            // Save the JSON documentation to blob storage.
            _logger.LogDebug("Saving json for {idver} target {target}", idver, target);
            var stopwatch = Stopwatch.StartNew();
            var jsonUri = await _storage.WriteJsonAsync(idver, target, docJson).ConfigureAwait(false);
            _logger.LogDebug("Saved json for {idver} target {target} at {url} in {elapsed}", idver, target, jsonUri, stopwatch.Elapsed);

            // Save the processing log to blob storage.
            var logUri = await SaveLogAsync(idver, target, success: true).ConfigureAwait(false);

            // Mark the processing as complete.
            await _table.SetRecordAsync(idver, target, Status.Succeeded, logUri, jsonUri).ConfigureAwait(false);
        }

        /// <summary>
        /// Writes a failure notification to storage.
        /// </summary>
        /// <param name="idver">The id and version of the package.</param>
        /// <param name="target">The target for the package.</param>
        public async Task WriteFailureAsync(NugetPackageIdVersion idver, PlatformTarget target)
        {
            // Save the processing log to blob storage.
            var logUri = await SaveLogAsync(idver, target, success: false).ConfigureAwait(false);

            // Mark the processing as failed.
            await _table.SetRecordAsync(idver, target, Status.Failed, logUri, null).ConfigureAwait(false);
        }

        private async Task<Uri> SaveLogAsync(NugetPackageIdVersion idver, PlatformTarget target, bool success)
        {
            var log = AmbientContext.InMemoryLoggerProvider?.Messages;
            var logJson = JsonConvert.SerializeObject(log, Constants.StorageJsonSerializerSettings);
            return await _storage.WriteLogAsync(idver, target, logJson, success).ConfigureAwait(false);
        }
    }
}
