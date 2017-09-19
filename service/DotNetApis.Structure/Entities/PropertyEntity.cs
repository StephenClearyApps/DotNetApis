using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Structure.TypeReferences;
using DotNetApis.Structure.Xmldoc;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Entities
{
    /// <summary>
    /// Structured documentation for a property.
    /// </summary>
    public sealed class PropertyEntity : IEntity, IHaveExplicitInterface
    {
        public EntityKind Kind => EntityKind.Property;
        public string DnaId { get; set; }
        public string Name { get; set; }
        public IReadOnlyList<AttributeJson> Attributes { get; set; }
        public EntityAccessibility Accessibility { get; set; }
        public EntityModifiers Modifiers { get; set; }
        public Xmldoc.Xmldoc Xmldoc { get; set; }
        public ITypeReference ExplicitInterfaceDeclaringType { get; set; }

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
