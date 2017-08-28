using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
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
        /// <param name="data">The gzipped log data.</param>
        /// <param name="dataLength">The length of <paramref name="data"/>.</param>
        Task<string> WriteAsync(NugetPackageIdVersion idver, PlatformTarget target, byte[] data, int dataLength);

        /// <summary>
        /// Gets the direct-access URI for a request's log data.
        /// </summary>
        /// <param name="blobPath">The path of the blob containing the log.</param>
        Uri GetUri(string blobPath);
    }

    public sealed class LogStorage : ILogStorage
    {
        private readonly CloudBlobContainer _container;

        public LogStorage(AzureConnections connections)
        {
            _container = GetContainer(connections);
        }

        private static CloudBlobContainer GetContainer(AzureConnections connections) => connections.CloudBlobClient.GetContainerReference("generatelogs");

        public static Task InitializeAsync(AzureConnections connections) => GetContainer(connections).CreateIfNotExistsAsync();

        public Uri GetUri(string blobPath) => _container.GetBlockBlobReference(blobPath).Uri;

        public async Task<string> WriteAsync(NugetPackageIdVersion idver, PlatformTarget target, byte[] data, int dataLength)
        {
            var blobPath = GetBlobPath(idver, target);
            var blob = _container.GetBlockBlobReference(blobPath);
            await blob.UploadFromByteArrayAsync(data, 0, dataLength);
            blob.Properties.CacheControl = "public, max-age=31536000";
            blob.Properties.ContentType = "text/plain; charset=utf-8";
            blob.Properties.ContentEncoding = "gzip";
            await blob.SetPropertiesAsync().ConfigureAwait(false);
            return blobPath;
        }

        private static string GetBlobPath(NugetPackageIdVersion idver, PlatformTarget target) => idver.PackageId + "/" + idver.Version + "/" + target + "/" + DateTimeOffset.UtcNow.ToString("s") + ".txt";
    }
}
