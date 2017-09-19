using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Structure.Literals;
using DotNetApis.Structure.TypeReferences;
using DotNetApis.Structure.Xmldoc;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Entities
{
    /// <summary>
    /// A parameter of a method.
    /// </summary>
    public sealed class MethodParameter
    {
        /// <summary>
        /// Attributes on the parameter.
        /// </summary>
        [JsonProperty("b")]
        public IReadOnlyList<AttributeJson> Attributes { get; set; }

        /// <summary>
        /// Modifiers for the parameter.
        /// </summary>
        [JsonProperty("m")]
        public MethodParameterModifiers Modifiers { get; set; }

        /// <summary>
        /// The type of the parameter.
        /// </summary>
        [JsonProperty("t")]
        public ITypeReference Type { get; set; }

        /// <summary>
        /// Name of the parameter.
        /// </summary>
        [JsonProperty("n")]
        public string Name { get; set; }

        /// <summary>
        /// Default value of the parameter, if any.
        /// </summary>
        [JsonProperty("v")]
        public ILiteral Value { get; set; }

        /// <summary>
        /// Xml documentation for the parameter.
        /// </summary>
        [JsonProperty("x")]
        public Xmldoc.Xmldoc Xmldoc { get; set; }
    }
}
