using DotNetApis.Structure.Locations;
using Newtonsoft.Json;

namespace DotNetApis.Structure.TypeReferences
{
    /// <summary>
    /// A type reference associated with a C# keyword.
    /// </summary>
    public sealed class KeywordTypeReference : ITypeReference
    {
        public TypeReferenceKind Kind => TypeReferenceKind.Keyword;

        /// <summary>
        /// The name of the keyword, e.g., "int".
        /// </summary>
        [JsonProperty("n")]
        public string Name { get; set; }

        /// <summary>
        /// The location of the actual type, e.g., the location of System.Int32.
        /// </summary>
        [JsonProperty("l")]
        public ILocation Location { get; set; }

        public override string ToString() => Name;
    }
}
