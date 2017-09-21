using Newtonsoft.Json;

namespace DotNetApis.Structure.TypeReferences
{
    /// <summary>
    /// Structured documentation for a type reference to a generic parameter, e.g., the <c>T</c> in <c>List&lt;T&gt;</c>.
    /// </summary>
    public sealed class GenericParameterTypeReference : ITypeReference
    {
        public TypeReferenceKind Kind => TypeReferenceKind.GenericParameter;

        /// <summary>
        /// The name of the generic parameter.
        /// </summary>
        [JsonProperty("n")]
        public string Name { get; set; }

        public override string ToString() => Name;
    }
}
