using DotNetApis.Structure.TypeReferences;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Literals
{
    /// <summary>
    /// A literal "typeof(T)" expression.
    /// </summary>
    public sealed class TypeofLiteral : ILiteral
    {
        public LiteralKind Kind => LiteralKind.Typeof;

        /// <summary>
        /// The type being referred to.
        /// </summary>
        [JsonProperty("t")]
        public ITypeReference Type { get; set; }
    }
}
