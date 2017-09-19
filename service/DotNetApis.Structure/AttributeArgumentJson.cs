using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.Structure
{
    /// <summary>
    /// Structured documentation for an attribute argument.
    /// </summary>
    public sealed class AttributeArgumentJson
    {
        /// <summary>
        /// The name of the parameter. May be <c>null</c> if this is a positional argument.
        /// </summary>
        [JsonProperty("n")]
        public string Name { get; set; }

        /// <summary>
        /// The attribute argument value.
        /// </summary>
        [JsonProperty("v")]
        public ILiteral Value { get; set; }
    }
}
