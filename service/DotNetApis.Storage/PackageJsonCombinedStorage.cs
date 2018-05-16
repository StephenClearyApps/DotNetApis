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
        /// <param name="doc">The documentation writer.</param>
        /// <param name="log">The log writer.</param>
        public async Task WriteSuccessAsync(NugetPackageIdVersion idver, PlatformTarget target, IBlobWriter doc, IBlobWriter log)
        {
            // Save the JSON documentation to blob storage.
            await doc.CommitAsync().ConfigureAwait(false);
            _logger.SavedJson(idver, target, doc.Uri);

            // Save the processing log to blob storage.
            await SaveLogAsync(log).ConfigureAwait(false);

            // Mark the processing as complete.
            await _table.SetRecordAsync(idver, target, Status.Succeeded, log.Uri, doc.Uri).ConfigureAwait(false);
        }

        /// <summary>
        /// Writes a failure notification to storage.
        /// </summary>
        /// <param name="idver">The id and version of the package.</param>
        /// <param name="target">The target for the package.</param>
        /// <param name="log">The log writer.</param>
        public async Task WriteFailureAsync(NugetPackageIdVersion idver, PlatformTarget target, IBlobWriter log)
        {
            // Save the processing log to blob storage.
            await SaveLogAsync(log).ConfigureAwait(false);

            // Mark the processing as failed.
            await _table.SetRecordAsync(idver, target, Status.Failed, log.Uri, null).ConfigureAwait(false);
        }

        private static async Task SaveLogAsync(IBlobWriter log)
        {
            await (AmbientContext.JsonLoggerProvider?.StopAsync() ?? Task.CompletedTask).ConfigureAwait(false);
            await log.CommitAsync().ConfigureAwait(false);
        }
    }

    internal static partial class Logging
    {
        public static void SavedJson(this ILogger<PackageJsonCombinedStorage> logger, NugetPackageIdVersion idver, PlatformTarget target, Uri uri) =>
            Logger.Log(logger, 1, LogLevel.Debug, "Saved json for {idver} target {target} at {uri}", idver, target, uri, null);
    }
}
