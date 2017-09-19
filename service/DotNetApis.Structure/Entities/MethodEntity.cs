using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Structure.TypeReferences;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Entities
{
    /// <summary>
    /// Structured documentation for a method.
    /// </summary>
    public sealed class MethodEntity : IEntity
    {
        public EntityKind Kind => EntityKind.Method;
        public string DnaId { get; set; }
        public string Name { get; set; }
        public IReadOnlyList<AttributeJson> Attributes { get; set; }
        public StructuredXmldoc Xmldoc { get; set; }

        /// <summary>
        /// Method accessibility.
        /// </summary>
        [JsonProperty("a")]
        public EntityAccessibility Accessibility { get; set; }

        /// <summary>
        /// Method modifiers.
        /// </summary>
        [JsonProperty("m")]
        public EntityModifiers Modifiers { get; set; }

        /// <summary>
        /// Method styles.
        /// </summary>
        [JsonProperty("s")]
        public MethodStyles Styles { get; set; }

        /// <summary>
        /// Return type of the method.
        /// </summary>
        [JsonProperty("r")]
        public ITypeReference ReturnType { get; set; }

        /// <summary>
        /// The method parameters.
        /// </summary>
        [JsonProperty("p")]
        public IReadOnlyList<MethodParameter> Parameters { get; set; }

        /// <summary>
        /// If the method is an explicit interface definition, then this is the interface.
        /// </summary>
        [JsonProperty("d")]
        public ITypeReference ExplicitInterfaceDeclaringType { get; set; }

        /// <summary>
        /// Generic parameters of the method.
        /// </summary>
        [JsonProperty("g")]
        public IReadOnlyList<GenericParameter> GenericParameters { get; set; }
    }
}
