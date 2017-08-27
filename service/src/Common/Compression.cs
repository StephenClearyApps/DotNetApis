using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Common
{
    /// <summary>
    /// Helper methods for zipping data stored in blobs.
    /// </summary>
    public static class Compression
    {
        /// <summary>
        /// GZips a string in a way that is appropriate for serving directly as <c>Content-Encoding: gzip</c> with <c>Content-Type</c> encoding <c>utf8</c>.
        /// </summary>
        /// <param name="data">The data to compress.</param>
        /// <param name="logger">The logger to log compression statistics to. May be <c>null</c>.</param>
        /// <returns></returns>
        public static (byte[], int) GzipString(string data, ILogger logger)
        {
            logger?.LogDebug("Before compression, string is {length} characters", data.Length);
            var dataBytes = Constants.Utf8.GetBytes(data);
            logger?.LogDebug("Before compression, string is {length} bytes", dataBytes.Length);
            using (var stream = new MemoryStream())
            {
                using (var gzip = new GZipStream(stream, CompressionMode.Compress, leaveOpen: true))
                    gzip.Write(dataBytes, 0, dataBytes.Length);
                var length = (int) stream.Position;
                logger?.LogDebug("After compression, string is {length} bytes", length);
                return (stream.GetBuffer(), length);
            }
        }
    }
}
