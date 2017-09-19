using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.Structure
{
    /// <summary>
    /// Structured documentation for an attribute.
    /// </summary>
    public sealed class AttributeJson
    {
        /// <summary>
        /// The target of the attribute, e.g., "assembly" or "return". Usually <c>null</c> to indicate the default target.
        /// </summary>
        [JsonProperty("t")]
        public string Target { get; set; }

        /// <summary>
        /// The short name of the attribute, without the namespace and without the "Attribute" suffix.
        /// </summary>
        [JsonProperty("n")]
        public string Name { get; set; }

        /// <summary>
        /// Location of the attribute type.
        /// </summary>
        [JsonProperty("l")]
        public ILocation Location { get; set; }

        /// <summary>
        /// The attribute constructor arguments.
        /// </summary>
        [JsonProperty("a")]
        public IReadOnlyList<AttributeArgumentJson> Arguments { get; set; }
    }
}
