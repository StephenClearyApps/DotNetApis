using DotNetApis.Structure.Locations;
using Newtonsoft.Json;

namespace DotNetApis.Structure.TypeReferences
{
    /// <summary>
    /// Structured documentation for a reqmod wrapper around an inner type reference.
    /// </summary>
    public sealed class ReqmodTypeReference : ITypeReference
    {
        public TypeReferenceKind Kind => TypeReferenceKind.Reqmod;

        /// <summary>
        /// The location of the reqmod type.
        /// </summary>
        [JsonProperty("l")]
        public ILocation Location { get; set; }

        /// <summary>
        /// The inner type reference that is wrapped by this reqmod type.
        /// </summary>
        [JsonProperty("t")]
        public ITypeReference ElementType { get; set; }
    }
}
