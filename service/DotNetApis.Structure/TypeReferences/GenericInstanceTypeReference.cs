using System.Collections.Generic;
using Newtonsoft.Json;

namespace DotNetApis.Structure.TypeReferences
{
    /// <summary>
    /// Structured documentation for a generic instance (a fully referenced concrete generic type).
    /// </summary>
    public sealed class GenericInstanceTypeReference : ITypeReference
    {
        public TypeReferenceKind Kind => TypeReferenceKind.GenericInstance;

        /// <summary>
        /// All declaring types and this type, as generic concrete types.
        /// </summary>
        [JsonProperty("t")]
        public IReadOnlyList<GenericConcreteType> DeclaringTypesAndThis { get; set; }

        public override string ToString() => string.Join(".", DeclaringTypesAndThis);
    }
}
