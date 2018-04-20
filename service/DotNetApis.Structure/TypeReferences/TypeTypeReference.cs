using DotNetApis.Structure.Locations;
using Newtonsoft.Json;

namespace DotNetApis.Structure.TypeReferences
{
    /// <summary>
    /// Structured documentation for a fully-qualified type reference. This may be a simple type name or it may be an open generic type.
    /// </summary>
    public sealed class TypeTypeReference : ITypeReference
    {
        public TypeReferenceKind Kind => TypeReferenceKind.Type;

        /// <summary>
        /// The name of the type, without a backtick suffix.
        /// </summary>
        [JsonProperty("n")]
        public string Name { get; set; }

        /// <summary>
        /// The namespace containing this type, if any.
        /// </summary>
        [JsonProperty("s")]
        public string Namespace { get; set; }

        /// <summary>
        /// The type where this type is declared, if any.
        /// </summary>
        [JsonProperty("t")]
        public ITypeReference DeclaringType { get; set; }

        /// <summary>
        /// The location of this type.
        /// </summary>
        [JsonProperty("l")]
        public ILocation Location { get; set; }

        /// <summary>
        /// The number of generic arguments, if this is an open generic type. If this is not an open generic type, this value is <c>0</c>.
        /// </summary>
        [JsonProperty("a")]
        public int GenericArgumentCount { get; set; }

        public override string ToString()
        {
            var result = Namespace ?? DeclaringType.ToString();
            result = result == "" ? Name : result + "." + Name;
            if (GenericArgumentCount != 0)
            {
                result += "<" + new string(',', GenericArgumentCount - 1) + ">";
            }
            return result;
        }
    }
}
