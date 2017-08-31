using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using Microsoft.WindowsAzure.Storage.Table;
using Nuget;
using Constants = Common.Constants;

namespace Storage
{
    public interface ILogStorage
    {
        /// <summary>
        /// Writes log data to storage and returns the blob path.
        /// </summary>
        /// <param name="idver">The id and version of the package.</param>
        /// <param name="target">The target for the package.</param>
        /// <param name="log">The log data.</param>
        Task<string> WriteAsync(DateTimeOffset originalRequestTimestamp, NugetPackageIdVersion idver, PlatformTarget target, string log);

        /// <summary>
        /// Gets the direct-access URI for a request's log data.
        /// </summary>
        /// <param name="blobPath">The path of the blob containing the log.</param>
        Uri GetUri(string blobPath);
    }

    public sealed class LogStorage : ILogStorage
    {
        private readonly AzureConnections _connections;

        public LogStorage(AzureConnections connections)
        {
            _connections = connections;
        }

        public Uri GetUri(string blobPath) => _container.GetBlockBlobReference(blobPath).Uri;

        public async Task<string> WriteAsync(DateTimeOffset originalRequestTimestamp, NugetPackageIdVersion idver, PlatformTarget target, Guid operationId, string log)
        {
            // Create table and container if necessary.
            var table = _connections.CloudTableClient.GetTableReference("log-" + originalRequestTimestamp.ToString("yyyy-MM-dd"));
            var container = _connections.CloudBlobClient.GetContainerReference("log-" + originalRequestTimestamp.ToString("yyyy-MM-dd"));
            await Task.WhenAll(table.CreateIfNotExistsAsync(), container.CreateIfNotExistsAsync()).ConfigureAwait(false);

            // Upload to blob.
            var blobPath = GetBlobPath(idver, target, operationId);
            var blob = container.GetBlockBlobReference(blobPath);
            var (data, dataLength) = Compression.GzipString(log, logger: null);
            await blob.UploadFromByteArrayAsync(data, 0, dataLength);
            blob.Properties.CacheControl = "public, max-age=31536000";
            blob.Properties.ContentType = "text/plain; charset=utf-8";
            blob.Properties.ContentEncoding = "gzip";
            await blob.SetPropertiesAsync().ConfigureAwait(false);

            // Save in table.
            table.ExecuteAsync(TableOperation.InsertOrMerge())

            // Blob: container=log-yyyy-mm-dd, {id}/{ver}/{target}/{operationId}.txt
            // Table: name=log-yyyy-mm-dd, PartitionKey=operationId, RowKey=operationId, id, ver, target, result, logblobpath


            return blobPath;
        }

        private static string GetBlobPath(NugetPackageIdVersion idver, PlatformTarget target, Guid operationId) => idver.PackageId + "/" + idver.Version + "/" + target + "/" + operationId.ToString("N") + ".txt";
    }
}
