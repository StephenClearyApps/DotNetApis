using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.Structure
{
    /// <summary>
    /// Structured documentation for a generic instance (a fully referenced concrete generic type).
    /// </summary>
    public sealed class GenericInstanceTypeReference : ITypeReference
    {
        public EntityReferenceKind Kind => EntityReferenceKind.GenericInstance;

        /// <summary>
        /// All declaring types and this type, as generic concrete types.
        /// </summary>
        [JsonProperty("t")]
        public IReadOnlyList<GenericConcreteType> DeclaringTypesAndThis { get; set; }
    }
}
