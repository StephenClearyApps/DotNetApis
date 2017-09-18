using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.Structure
{
    /// <summary>
    /// Structured documentation for a type reference to a generic parameter, e.g., the <c>T</c> in <c>List&lt;T&gt;</c>.
    /// </summary>
    public sealed class GenericParameterTypeReference : TypeReferenceBase
    {
        public override EntityReferenceKind Kind => EntityReferenceKind.GenericParameter;

        /// <summary>
        /// The name of the generic parameter.
        /// </summary>
        [JsonProperty("n")]
        public string Name { get; set; }
    }
}
