using System;
using System.Threading.Tasks;
using DotNetApis.Nuget;

namespace DotNetApis.Storage
{
    public sealed class LogCombinedStorage
    {
        private readonly ILogStorage _storage;
        private readonly IStatusTable _table;

        public LogCombinedStorage(ILogStorage storage, IStatusTable table)
        {
            _storage = storage;
            _table = table;
        }

        /// <summary>
        /// Writes results and log data to storage.
        /// </summary>
        /// <param name="idver">The id and version of the package.</param>
        /// <param name="target">The target for the package.</param>
        /// <param name="timestamp">The timestamp of the original documentation request.</param>
        /// <param name="status">The result of the request.</param>
        /// <param name="log">The log data.</param>
        /// <param name="jsonUri">The URI to the resulting JSON. May be <c>null</c>.</param>
        public async Task WriteAsync(NugetPackageIdVersion idver, PlatformTarget target, DateTimeOffset timestamp, Status status, string log, Uri jsonUri)
        {
            var logUri = await _storage.WriteAsync(idver, target, timestamp, log).ConfigureAwait(false);
            await _table.WriteStatusAsync(idver, target, timestamp, status, logUri, jsonUri).ConfigureAwait(false);
        }
    }
}
