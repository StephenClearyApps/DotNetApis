using System.Collections.Generic;
using Newtonsoft.Json;

namespace DotNetApis.Structure.TypeReferences
{
    /// <summary>
    /// Structured documentation for an array type reference.
    /// </summary>
    public sealed class ArrayTypeReference : ITypeReference
    {
        public TypeReferenceKind Kind => TypeReferenceKind.Array;

        /// <summary>
        /// The type of the elements of the array.
        /// </summary>
        [JsonProperty("t")]
        public ITypeReference ElementType { get; set; }

        /// <summary>
        /// The dimensions of the array.
        /// </summary>
        [JsonProperty("d")]
        public IReadOnlyList<ArrayDimensionJson> Dimensions { get; set; }

        public override string ToString() => ElementType + "[]";
    }
}
