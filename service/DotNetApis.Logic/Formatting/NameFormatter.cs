using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetApis.Logic.Formatting
{
    /// <summary>
    /// Formats simple names.
    /// </summary>
    public sealed class NameFormatter
    {
        /// <summary>
        /// All C# keywords, including any contextual keywords that could interfere with identifiers used in declarations.
        /// </summary>
        private static readonly string[] KnownCsharpKeywords =
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do",
            "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface",
            "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public",
            "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true",
            "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while",

            "await", "dynamic", "value",
        };

        /// <summary>
        /// Prefixes an identifier with an "@" if necessary.
        /// </summary>
        /// <param name="identifier">The identifier to escape. May be <c>null</c>.</param>
        public string EscapeIdentifier(string identifier) => KnownCsharpKeywords.Contains(identifier) ? "@" + identifier : identifier;
    }
}
