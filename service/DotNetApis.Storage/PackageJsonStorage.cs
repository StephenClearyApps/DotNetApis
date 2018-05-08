using System;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Nuget;
using DotNetApis.Structure;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DotNetApis.Storage
{
    public interface IPackageJsonStorage
    {
        /// <summary>
        /// Writes JSON data to storage and returns the direct-access URI.
        /// </summary>
        /// <param name="idver">The id and version of the package.</param>
        /// <param name="target">The target for the package.</param>
        /// <param name="json">The JSON documentation for the specified package id, version, and target.</param>
        Task<Uri> WriteJsonAsync(NugetPackageIdVersion idver, PlatformTarget target, string json);

        /// <summary>
        /// Writes log data to storage and returns the direct-access URI.
        /// </summary>
        /// <param name="idver">The id and version of the package.</param>
        /// <param name="target">The target for the package.</param>
        /// <param name="json">The JSON documentation for the specified package id, version, and target.</param>
        /// <param name="success">The "slot" in which to save the log, whether successful or failed.</param>
        Task<Uri> WriteLogAsync(NugetPackageIdVersion idver, PlatformTarget target, string json, bool success);
    }

    public sealed class AzurePackageJsonStorage : IPackageJsonStorage
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly CloudBlobContainer _container;

        public static string ContainerName { get; } = "packagejson" + JsonFactory.Version;

        public AzurePackageJsonStorage(ILoggerFactory loggerFactory, CloudBlobContainer container)
        {
	        _loggerFactory = loggerFactory;
            _container = container;
        }

        public async Task<Uri> WriteJsonAsync(NugetPackageIdVersion idver, PlatformTarget target, string json)
        {
            var (data, dataLength) = Compression.GzipString(json, _loggerFactory);
            var blobPath = GetJsonBlobPath(idver, target);
            var blob = _container.GetBlockBlobReference(blobPath);
            await blob.UploadFromByteArrayAsync(data, 0, dataLength).ConfigureAwait(false);
            blob.Properties.CacheControl = "public, max-age=31536000";
            blob.Properties.ContentType = "application/json; charset=utf-8";
            blob.Properties.ContentEncoding = "gzip";
            await blob.SetPropertiesAsync().ConfigureAwait(false);
            return blob.Uri;
        }

        public async Task<Uri> WriteLogAsync(NugetPackageIdVersion idver, PlatformTarget target, string log, bool success)
        {
            var (data, dataLength) = Compression.GzipString(log, _loggerFactory);
            var blobPath = GetLogBlobPath(idver, target, success);
            var blob = _container.GetBlockBlobReference(blobPath);
            await blob.UploadFromByteArrayAsync(data, 0, dataLength).ConfigureAwait(false);
            blob.Properties.ContentType = "application/json; charset=utf-8";
            blob.Properties.ContentEncoding = "gzip";
            await blob.SetPropertiesAsync().ConfigureAwait(false);
            return blob.Uri;
        }

        private static string GetJsonBlobPath(NugetPackageIdVersion idver, PlatformTarget target) => idver.PackageId + "/" + idver.Version + "/" + target + ".json";

        private static string GetLogBlobPath(NugetPackageIdVersion idver, PlatformTarget target, bool success) => idver.PackageId + "/" + idver.Version + "/" + target + "." + (success ? "log" : "error") + ".json";
    }
}
