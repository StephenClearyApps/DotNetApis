using System.IO;
using System.IO.Compression;
using Microsoft.Extensions.Logging;

namespace DotNetApis.Common
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
        public static (byte[], int) GzipString(string data, ILoggerFactory loggerFactory)
        {
	        var logger = loggerFactory?.CreateLogger<Logging.Compression>();

            var dataBytes = Constants.Utf8.GetBytes(data);
            using (var stream = new MemoryStream())
            {
                using (var gzip = new GZipStream(stream, CompressionMode.Compress, leaveOpen: true))
                    gzip.Write(dataBytes, 0, dataBytes.Length);
                var length = (int) stream.Position;
                logger?.Stats(data.Length, dataBytes.Length, length);
                return (stream.GetBuffer(), length);
            }
        }
    }

	internal static partial class Logging
	{
		public static void Stats(this ILogger<Compression> logger, int uncompressedCharacters, int uncompressedBytes, int compressedBytes) =>
			Logger.Log(logger, 1, LogLevel.Debug, "Before compression, string was {uncompressedBytes} bytes ({uncompressedCharacters} chars); after compression, string is {compressedBytes} bytes",
				uncompressedBytes, uncompressedCharacters, compressedBytes, null);

		public sealed class Compression { }
	}
}
