using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.Structure
{
    /// <summary>
    /// Structured documentation for an array type reference.
    /// </summary>
    public sealed class ArrayTypeReference : ITypeReference
    {
        public EntityReferenceKind Kind => EntityReferenceKind.Array;

        /// <summary>
        /// The type of the elements of the array.
        /// </summary>
        [JsonProperty("t")]
        public ITypeReference ElementType { get; set; }

        /// <summary>
        /// The dimensions of the array.
        /// </summary>
        [JsonProperty("d")]
        public IReadOnlyList<ArrayDimension> Dimensions { get; set; }
    }
}
