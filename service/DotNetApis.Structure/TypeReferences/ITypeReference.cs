using Newtonsoft.Json;

namespace DotNetApis.Structure.TypeReferences
{
    /// <summary>
    /// Structured documentation for a type reference.
    /// </summary>
    public interface ITypeReference
    {
        /// <summary>
        /// The kind of type reference this is.
        /// </summary>
        [JsonProperty("k")]
        EntityReferenceKind Kind { get; }
    }
}
