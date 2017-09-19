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
    /// Structured documentation for an event.
    /// </summary>
    public sealed class EventEntity : IEntity
    {
        public EntityKind Kind => EntityKind.Event;
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
        /// Type of the event.
        /// </summary>
        [JsonProperty("t")]
        public ITypeReference Type { get; set; }

        /// <summary>
        /// If the event is an explicit interface definition, then this is the interface.
        /// </summary>
        [JsonProperty("d")]
        public ITypeReference ExplicitInterfaceDeclaringType { get; set; }

        /// <summary>
        /// Attributes on the add method.
        /// </summary>
        [JsonProperty("p")]
        public IReadOnlyList<AttributeJson> AddMethodAttributes { get; set; }

        /// <summary>
        /// Attributes on the remove method.
        /// </summary>
        [JsonProperty("r")]
        public IReadOnlyList<AttributeJson> RemoveMethodAttributes { get; set; }
    }
}
