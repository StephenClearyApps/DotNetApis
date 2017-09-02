using System;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Nuget;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DotNetApis.Storage
{
    public interface IPackageJsonStorage
    {
        /// <summary>
        /// Writes JSON data to storage and returns the blob path.
        /// </summary>
        /// <param name="idver">The id and version of the package.</param>
        /// <param name="target">The target for the package.</param>
        /// <param name="json">The JSON documentation for the specified package id, version, and target.</param>
        Task<string> WriteAsync(NugetPackageIdVersion idver, PlatformTarget target, string json);

        /// <summary>
        /// Gets the direct-access URI for a package's JSON.
        /// </summary>
        /// <param name="idver">The id and version of the package.</param>
        /// <param name="target">The target for the package.</param>
        Uri GetUri(NugetPackageIdVersion idver, PlatformTarget target);
    }

    public sealed class AzurePackageJsonStorage : IPackageJsonStorage
    {
        private readonly ILogger _logger;
        private readonly CloudBlobContainer _container;

        public AzurePackageJsonStorage(ILogger logger, AzureConnections connections)
        {
            _logger = logger;
            _container = GetContainer(connections);
        }

        private static CloudBlobContainer GetContainer(AzureConnections connections) => connections.CloudBlobClient.GetContainerReference("packagejson" + JsonFactory.Version);

        public static Task InitializeAsync(AzureConnections connections) => GetContainer(connections).CreateIfNotExistsAsync();

        public Uri GetUri(NugetPackageIdVersion idver, PlatformTarget target) => _container.GetBlockBlobReference(GetBlobPath(idver, target)).Uri;

        public async Task<string> WriteAsync(NugetPackageIdVersion idver, PlatformTarget target, string json)
        {
            var (data, dataLength) = Compression.GzipString(json, _logger);
            var blobPath = GetBlobPath(idver, target);
            var blob = _container.GetBlockBlobReference(blobPath);
            await blob.UploadFromByteArrayAsync(data, 0, dataLength).ConfigureAwait(false);
            blob.Properties.CacheControl = "public, max-age=31536000";
            blob.Properties.ContentType = "application/json; charset=utf-8";
            blob.Properties.ContentEncoding = "gzip";
            await blob.SetPropertiesAsync().ConfigureAwait(false);
            _logger.LogDebug("Saved json for {idver} target {target} at {uri}", idver, target, blob.Uri);
            return blobPath;
        }

        private static string GetBlobPath(NugetPackageIdVersion idver, PlatformTarget target) => idver.PackageId + "/" + idver.Version + "/" + target + ".json";
    }
}
