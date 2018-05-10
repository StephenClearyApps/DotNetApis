using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Nuget;
using DotNetApis.Structure;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

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
		/// Opens a writer that streams JSON writes to a blob.
		/// </summary>
		/// <param name="idver">The id and version of the package.</param>
		/// <param name="target">The target for the package.</param>
	    Task<IJsonBlobWriter> OpenLogAsync(NugetPackageIdVersion idver, PlatformTarget target);
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

	    public async Task<IJsonBlobWriter> OpenLogAsync(NugetPackageIdVersion idver, PlatformTarget target)
	    {
		    var blobPath = GetLogBlobPath(idver, target);
		    var blob = _container.GetBlockBlobReference(blobPath);
		    var blobStream = await blob.OpenWriteAsync().ConfigureAwait(false);
		    return new BlobWriteContext(blob, blobStream);
	    }

		private static string GetJsonBlobPath(NugetPackageIdVersion idver, PlatformTarget target) => idver.PackageId + "/" + idver.Version + "/" + target + ".json";

	    private static string GetLogBlobPath(NugetPackageIdVersion idver, PlatformTarget target) => $"{idver.PackageId}/{idver.Version}/{target}/{Guid.NewGuid():N}.log.json";

	    private sealed class BlobWriteContext: IJsonBlobWriter
		{
		    private readonly CloudBlockBlob _blob;
		    private readonly CloudBlobStream _blobStream;
		    private readonly GZipStream _gzipStream;
		    private readonly StreamWriter _utf8StreamWriter;
		    private readonly JsonTextWriter _jsonTextWriter;

		    public BlobWriteContext(CloudBlockBlob blob, CloudBlobStream blobStream)
		    {
			    _blob = blob;
			    _blobStream = blobStream;
			    _gzipStream = new GZipStream(blobStream, CompressionMode.Compress, leaveOpen: true);
				_utf8StreamWriter = new StreamWriter(_gzipStream, Constants.Utf8);
			    _jsonTextWriter = new JsonTextWriter(_utf8StreamWriter);
		    }

		    public Uri Uri => _blob.Uri;

			public JsonWriter JsonWriter => _jsonTextWriter;

		    public async Task CommitAsync()
		    {
				// JsonTextWriter.FlushAsync is only in newer versions, which we can't use because of Azure Functions runtime.

				// Flush downstream first, since JsonTextWriter's flush has to be synchronous.
			    await _utf8StreamWriter.FlushAsync();
			    await _gzipStream.FlushAsync();
			    await _blobStream.FlushAsync();

				// Flush all the way through.
				_jsonTextWriter.Flush();
			    await _utf8StreamWriter.FlushAsync();
			    await _gzipStream.FlushAsync();
			    await _blobStream.FlushAsync();

				// Close all streams except the CloudBlobStream.
				_jsonTextWriter.Close();

			    // Commit the stream.
			    await Task.Factory.FromAsync(_blobStream.BeginCommit, _blobStream.EndCommit, null);

				// Set the blob properties.
				_blob.Properties.ContentType = "application/json; charset=utf-8";
			    _blob.Properties.ContentEncoding = "gzip";
			    await _blob.SetPropertiesAsync().ConfigureAwait(false);
			}
		}
    }
}
