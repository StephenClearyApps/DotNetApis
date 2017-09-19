using Newtonsoft.Json;

namespace DotNetApis.Structure.TypeReferences
{
    /// <summary>
    /// Structured documentation for a pointer type.
    /// </summary>
    public sealed class PointerTypeReference : ITypeReference
    {
        public TypeReferenceKind Kind => TypeReferenceKind.Pointer;

        /// <summary>
        /// The type that this is a pointer to.
        /// </summary>
        [JsonProperty("t")]
        public ITypeReference ElementType { get; set; }
    }
}
