using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

// Temporarily copied into this project until https://github.com/Azure/Azure-Functions/issues/477 is reolved

namespace Nito.UniformResourceIdentifiers.Implementation
{
    /// <summary>
    /// Utility constants and methods useful for constructing URI parsers and builders conforming to RFC3986 (https://tools.ietf.org/html/rfc3986).
    /// </summary>
    public static class UriUtil
    {
        /// <summary>
        /// The standard UTF8 encoding.
        /// </summary>
        public static Encoding Utf8EncodingWithoutBom { get; } = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        /// <summary>
        /// Percent-encodes a single character.
        /// </summary>
        /// <param name="ch">The character to encode.</param>
        public static string PercentEncode(byte ch) => "%" + ch.ToString("X2", CultureInfo.InvariantCulture);

        /// <summary>
        /// Percent-encodes the string <paramref name="value"/> except for those characters for which <paramref name="isSafe"/> returns <c>true</c>.
        /// </summary>
        /// <param name="value">The value to encode.</param>
        /// <param name="isSafe">A function determining whether a character is safe (i.e., does not need encoding).</param>
        public static string PercentEncode(string value, Func<byte, bool> isSafe)
        {
            var bytes = Utf8EncodingWithoutBom.GetBytes(value);
            var sb = new StringBuilder(bytes.Length);
            foreach (var ch in bytes)
            {
                if (isSafe(ch))
                    sb.Append((char)ch);
                else
                    sb.Append(PercentEncode(ch));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Percent-decodes the string <paramref name="value"/>, verifying that the only non-encoded characters are those for which <paramref name="isSafe"/> returns <c>true</c>.
        /// </summary>
        /// <param name="value">The value to decode.</param>
        /// <param name="isSafe">A function determining whether a character is safe (i.e., does not need encoding).</param>
        public static string PercentDecode(string value, Func<byte, bool> isSafe)
        {
            var sb = new StringBuilder(value.Length);
            for (var i = 0; i != value.Length; ++i)
            {
                var ch = value[i];
                if (ch >= 256)
                    throw new FormatException($"Invalid character \"{ch}\" at index {i} in string \"{value}\".");
                if (ch == '%')
                {
                    if (i + 2 >= value.Length)
                        throw new FormatException($"Unterminated percent-encoding at index {i} in string \"{value}\".");
                    var hexString = value.Substring(i + 1, 2);
                    if (!byte.TryParse(hexString, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out byte encodedValue))
                        throw new FormatException($"Invalid percent-encoding at index {i} in string \"{value}\".");
                    sb.Append((char)encodedValue);
                    i += 2;
                }
                else if (isSafe((byte)ch))
                {
                    sb.Append(ch);
                }
                else
                {
                    throw new FormatException($"Invalid character \"{ch}\" at index {i} in string \"{value}\".");
                }
            }
            return sb.ToString();
        }

        private static readonly Func<byte, bool> FormUrlIsSafe =
            b => (b >= 'a' && b <= 'z') || (b >= 'A' && b <= 'Z') || (b >= '0' && b <= '9') || b == '*' || b == '-' || b == '.' || b == '_' || b == ' ';

        /// <summary>
        /// Encodes a string using <c>application/x-www-form-urlencoded</c> (https://www.w3.org/TR/html5/forms.html#application/x-www-form-urlencoded-encoding-algorithm). Always uses UTF-8 encoding.
        /// </summary>
        /// <param name="value">The string to encode.</param>
        public static string FormUrlEncode(string value) => PercentEncode(value, FormUrlIsSafe).Replace(' ', '+');

        /// <summary>
        /// Encodes a sequence of name/value pairs using <c>application/x-www-form-urlencoded</c>. Always uses UTF-8 encoding, and does not do any special handling for known names (e.g., <c>_charset_</c>).
        /// </summary>
        /// <param name="values">The values to encode.</param>
        public static string FormUrlEncode(IEnumerable<KeyValuePair<string, string>> values)
        {
            return string.Join("&", values.Select(x => FormUrlEncode(x.Key) + "=" + FormUrlEncode(x.Value)));
        }

        /// <summary>
        /// Decodes an individual <c>application/x-www-form-urlencoded</c> string. Always uses UTF-8 encoding.
        /// </summary>
        /// <param name="value">The string to decode.</param>
        public static string FormUrlDecode(string value) => PercentDecode(value.Replace('+', ' '), FormUrlIsSafe);

        /// <summary>
        /// Decodes a sequence of name/value pairs using <c>application/x-www-form-urlencoded</c>. Always uses UTF-8 encoding.
        /// </summary>
        /// <param name="query">The query string to decode.</param>
        public static IEnumerable<KeyValuePair<string, string>> FormUrlDecodeValues(string query)
        {
            var pairs = query.Split('&');
            foreach (var pair in pairs)
            {
                if (pair == "")
                {
                    yield return new KeyValuePair<string, string>("", null);
                    continue;
                }
                var parts = pair.Split('=');
                if (parts.Length == 1)
                {
                    yield return new KeyValuePair<string, string>(FormUrlDecode(parts[0]), null);
                }
                else if (parts.Length == 2)
                {
                    yield return new KeyValuePair<string, string>(FormUrlDecode(parts[0]), FormUrlDecode(parts[1]));
                }
                else
                {
                    throw new FormatException($"More than one '=' in query expression {query}");
                }
            }
        }
    }
}
