using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.Structure
{
    /// <summary>
    /// Structured documentation for a generic type combined with its generic type arguments.
    /// </summary>
    public sealed class GenericConcreteType
    {
        /// <summary>
        /// The name of the generic type, without a backtick suffix.
        /// </summary>
        [JsonProperty("n")]
        public string Name { get; set; }

        /// <summary>
        /// The location of the generic type.
        /// </summary>
        [JsonProperty("l")]
        public ILocation Location { get; set; }

        /// <summary>
        /// The generic type arguments.
        /// </summary>
        [JsonProperty("a")]
        public IReadOnlyList<ITypeReference> Arguments { get; set; }
    }
}
