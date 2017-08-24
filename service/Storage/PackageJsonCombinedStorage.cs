using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;
using Nuget;

namespace Storage
{
    public sealed class PackageJsonCombinedStorage
    {
        private readonly ILogger _logger;
        private readonly IPackageJsonTable _table;
        private readonly IPackageJsonStorage _storage;

        public PackageJsonCombinedStorage(ILogger logger, IPackageJsonTable table, IPackageJsonStorage storage)
        {
            _logger = logger;
            _table = table;
            _storage = storage;
        }

        /// <summary>
        /// Writes a complete JSON string to storage and returns the direct-access URI for that JSON.
        /// </summary>
        /// <param name="idver">The id and version of the package.</param>
        /// <param name="target">The target for the package.</param>
        /// <param name="json">The JSON documentation for the specified package id, version, and target.</param>
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
            var blobPath = await _storage.WriteAsync(idver, target, raw, rawLength);
            await _table.SetBlobPathAsync(idver, target, blobPath);
            return _storage.GetUri(idver, target);
        }

        /// <summary>
        /// Returns the direct-access URI for a package's JSON. Returns <c>null</c> if the package hasn't been processed yet.
        /// </summary>
        /// <param name="idver">The id and version of the package.</param>
        /// <param name="target">The target for the package.</param>
        public async Task<Uri> TryGetUriAsync(NugetPackageIdVersion idver, PlatformTarget target)
        {
            // Do a point lookup; if the package hasn't been processed yet, then return null.
            if (await _table.TryGetBlobPathAsync(idver, target).ConfigureAwait(false) == null)
                return null;

            return _storage.GetUri(idver, target);
        }
    }
}
