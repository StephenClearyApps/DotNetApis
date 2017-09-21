using System.Collections.Generic;
using DotNetApis.Structure.TypeReferences;
using DotNetApis.Structure.Util;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Literals
{
    /// <summary>
    /// A literal enumeration value.
    /// </summary>
    public sealed class EnumLiteral : ILiteral
    {
        public LiteralKind Kind => LiteralKind.Enum;

        /// <summary>
        /// The numeric value of the enumeration value. This may be a byte, sbyte, short, ushort, int, uint, long, or ulong.
        /// </summary>
        [JsonProperty("v")]
        public object Value { get; set; }

        /// <summary>
        /// Whether the context of this literal value implies it should be displayed in hexadecimal rather than decimal (only applies to integral values).
        /// </summary>
        [JsonProperty("h"), JsonConverter(typeof(IntBooleanConverter))]
        public bool PreferHex { get; set; }

        /// <summary>
        /// The type of the enumeration.
        /// </summary>
        [JsonProperty("t")]
        public ITypeReference EnumType { get; set; }

        /// <summary>
        /// The name(s) of the matching enumeration values, if any. If there is an exact match, this collection has one item; otherwise, if there are multiple flags that combine to this value, this collection has all of them; otherwise, this collection is empty.
        /// </summary>
        [JsonProperty("n")]
        public IReadOnlyList<string> Names { get; set; }

        public override string ToString() => Names.Count == 0 ? Value.ToString() : string.Join("|", Names);
    }
}
