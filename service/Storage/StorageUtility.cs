using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Storage
{
    public static class StorageUtility
    {
        /// <summary>
        /// Computes a SHA-1 hash for a source string and returns that hash as a hex string.
        /// </summary>
        /// <param name="source">The source string to hash.</param>
        public static string HashString(string source)
        {
            var bytes = Constants.Sha1.ComputeHash(Constants.Utf8.GetBytes(source));
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
                sb.Append(b.ToString("X2", CultureInfo.InvariantCulture));
            return sb.ToString();
        }
    }
}
