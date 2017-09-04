using System;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Nuget;
using DotNetApis.StructuredFormatter;

namespace DotNetApis.Storage
{
    public interface ILogStorage
    {
        /// <summary>
        /// Writes log data to storage and returns the direct-access blob URI.
        /// </summary>
        /// <param name="idver">The id and version of the package.</param>
        /// <param name="target">The target for the package.</param>
        /// <param name="timestamp">The timestamp of the original documentation request.</param>
        /// <param name="log">The log data.</param>
        Task<Uri> WriteAsync(NugetPackageIdVersion idver, PlatformTarget target, DateTimeOffset timestamp, string log);
    }

    public sealed class AzureLogStorage : ILogStorage
    {
        private readonly AzureConnections _connections;

        public AzureLogStorage(AzureConnections connections)
        {
            _connections = connections;
        }

        public async Task<Uri> WriteAsync(NugetPackageIdVersion idver, PlatformTarget target, DateTimeOffset timestamp, string log)
        {
            // Create container if necessary.
            var container = _connections.CloudBlobClient.GetContainerReference("log" + JsonFactory.Version + "-" + timestamp.ToString("yyyy-MM-dd"));
            await container.CreateIfNotExistsAsync().ConfigureAwait(false);

            // Upload to blob.
            var blobPath = GetBlobPath(idver, target);
            var blob = container.GetBlockBlobReference(blobPath);
            var (data, dataLength) = Compression.GzipString(log, logger: null);
            await blob.UploadFromByteArrayAsync(data, 0, dataLength);
            blob.Properties.CacheControl = "public, max-age=31536000";
            blob.Properties.ContentType = "text/plain; charset=utf-8";
            blob.Properties.ContentEncoding = "gzip";
            await blob.SetPropertiesAsync().ConfigureAwait(false);

            return blob.Uri;
        }

        private static string GetBlobPath(NugetPackageIdVersion idver, PlatformTarget target) => idver.PackageId + "/" + idver.Version + "/" + target + ".txt";
    }
}
