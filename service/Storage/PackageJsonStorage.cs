using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using Nuget;
using Constants = Common.Constants;

namespace Storage
{
    public interface IPackageJsonStorage
    {
        /// <summary>
        /// Writes a complete JSON string to storage and returns the direct-access URI for that JSON.
        /// </summary>
        /// <param name="idver">The id and version of the package.</param>
        /// <param name="target">The target for the package.</param>
        /// <param name="json">The JSON documentation for the specified package id, version, and target.</param>
        Task<Uri> WriteAsync(NugetPackageIdVersion idver, PlatformTarget target, string json);

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

        public static async Task InitializeAsync()
        {
            var connections = new AzureConnections();

            var client = connections.CloudBlobClient;
            var properties = await client.GetServicePropertiesAsync().ConfigureAwait(false);
            properties.Cors = new CorsProperties();
            properties.Cors.CorsRules.Add(new CorsRule
            {
                AllowedHeaders = { "*" },
                AllowedMethods = CorsHttpMethods.Get,
                AllowedOrigins = { "*" },
                ExposedHeaders = { "*" },
                MaxAgeInSeconds = 31536000,
            });
            await client.SetServicePropertiesAsync(properties).ConfigureAwait(false);

            await GetContainer(connections).CreateIfNotExistsAsync().ConfigureAwait(false);
        }

        public Uri GetUri(NugetPackageIdVersion idver, PlatformTarget target) => _container.GetBlockBlobReference(GetBlobPath(idver, target)).Uri;

        public async Task<Uri> WriteAsync(NugetPackageIdVersion idver, PlatformTarget target, string json)
        {
            _logger.LogDebug("Saving json for {idver} target {target}: JSON is {length} characters", idver, target, json.Length);
            var jsonData = Constants.Utf8.GetBytes(json);
            _logger.LogDebug("Saving json for {idver} target {target}: JSON is {length} bytes", idver, target, jsonData.Length);
            byte[] raw;
            int rawLength;
            using (var stream = new MemoryStream())
            {
                using (var gzip = new GZipStream(stream, CompressionMode.Compress, true))
                    gzip.Write(jsonData, 0, jsonData.Length);
                raw = stream.GetBuffer();
                rawLength = (int)stream.Position;
            }
            _logger.LogDebug("Saving json for {idver} target {target}: JSON is {length} bytes compressed", idver, target, rawLength);
            var blob = _container.GetBlockBlobReference(GetBlobPath(idver, target));
            await blob.UploadFromByteArrayAsync(raw, 0, rawLength).ConfigureAwait(false);
            blob.Properties.CacheControl = "public, max-age=31536000";
            blob.Properties.ContentType = "application/json; charset=utf-8";
            blob.Properties.ContentEncoding = "gzip";
            await blob.SetPropertiesAsync().ConfigureAwait(false);
            return blob.Uri;
        }

        private static string GetBlobPath(NugetPackageIdVersion idver, PlatformTarget target) => idver.PackageId + "/" + idver.Version + "/" + target + ".json";
    }
}
