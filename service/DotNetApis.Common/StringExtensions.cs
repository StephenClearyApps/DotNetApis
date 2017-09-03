using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetApis.Common
{
    public static class StringExtensions
    {
        /// <summary>
        /// @-encodes a string to a specific alphabet.
        /// </summary>
        /// <param name="text">The string.</param>
        /// <param name="alphabet">The alphabet of allowed characters.</param>
        public static string AtEncode(this string text, char[] alphabet)
        {
            // First to a quick check to avoid allocating a new string.
            if (text.IndexOfAny(alphabet) == -1)
                return text;

            var sb = new StringBuilder(text.Length);
            var bytes = Constants.Utf8.GetBytes(text);
            foreach (var b in bytes)
            {
                if (alphabet.Contains((char)b))
                    sb.Append((char)b);
                else
                    sb.Append("@" + b.ToString("X2", CultureInfo.InvariantCulture));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Removes the backtick suffix (if any) of the string. Works for double-backtick suffixed strings as well.
        /// </summary>
        public static (string Name, int Value) StripBacktickSuffix(this string s)
        {
            var backtickIndex = s.IndexOf('`');
            if (backtickIndex == -1)
                return (s, 0);
            return (s.Substring(0, backtickIndex), int.Parse(s.Substring(s.LastIndexOf('`') + 1), CultureInfo.InvariantCulture));
        }
    }
}
