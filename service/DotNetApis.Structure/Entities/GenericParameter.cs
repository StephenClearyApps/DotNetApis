using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Structure.GenericConstraints;
using DotNetApis.Structure.Xmldoc;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Entities
{
    /// <summary>
    /// Structured documentation for a generic parameter of a type or method.
    /// </summary>
    public sealed class GenericParameter
    {
        /// <summary>
        /// Modifiers for this parameter.
        /// </summary>
        [JsonProperty("m")]
        public GenericParameterModifiers Modifiers { get; set; }

        /// <summary>
        /// The name of this parameter.
        /// </summary>
        [JsonProperty("n")]
        public string Name { get; set; }

        /// <summary>
        /// The generic constraints for this parameter.
        /// </summary>
        [JsonProperty("c")]
        public IReadOnlyList<IGenericConstraint> GenericConstraints { get; set; }

        /// <summary>
        /// The xmldocs for this generic parameter.
        /// </summary>
        [JsonProperty("x")]
        public Xmldoc.Xmldoc Xmldoc { get; set; }
    }
}
