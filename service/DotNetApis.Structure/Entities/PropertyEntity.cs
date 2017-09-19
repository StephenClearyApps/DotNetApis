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
    /// Structured documentation for a property.
    /// </summary>
    public sealed class PropertyEntity : IEntity
    {
        public EntityKind Kind => EntityKind.Property;
        public string DnaId { get; set; }
        public string Name { get; set; }
        public IReadOnlyList<AttributeJson> Attributes { get; set; }
        public StructuredXmldoc Xmldoc { get; set; }

        /// <summary>
        /// Accessibility of the entity.
        /// </summary>
        [JsonProperty("a")]
        public EntityAccessibility Accessibility { get; set; }

        /// <summary>
        /// Modifiers of the entity.
        /// </summary>
        [JsonProperty("m")]
        public EntityModifiers Modifiers { get; set; }

        /// <summary>
        /// Type of the property.
        /// </summary>
        [JsonProperty("t")]
        public ITypeReference Type { get; set; }

        /// <summary>
        /// Parameters of the property, if this is an indexed property.
        /// </summary>
        [JsonProperty("p")]
        public IReadOnlyList<MethodParameter> Parameters { get; set; }

        /// <summary>
        /// If the property is an explicit interface definition, then this is the interface.
        /// </summary>
        [JsonProperty("d")]
        public ITypeReference ExplicitInterfaceDeclaringType { get; set; }

        /// <summary>
        /// Structured documentation of the property's get accessor.
        /// </summary>
        [JsonProperty("g")]
        public PropertyMethod GetMethod { get; set; }

        /// <summary>
        /// Structured documentation of the property's set accessor.
        /// </summary>
        [JsonProperty("s")]
        public PropertyMethod SetMethod { get; set; }
    }
}
