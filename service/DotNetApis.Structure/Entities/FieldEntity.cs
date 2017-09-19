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
    /// A field (or constant value).
    /// </summary>
    public sealed class FieldEntity : IEntity
    {
        public EntityKind Kind => EntityKind.Field;
        public string DnaId { get; set; }
        public string Name { get; set; }
        public EntityModifiers Modifiers { get; set; }
        public Xmldoc.Xmldoc Xmldoc { get; set; }
        public IReadOnlyList<AttributeJson> Attributes { get; set; }
        public EntityAccessibility Accessibility { get; set; }

        /// <summary>
        /// The type of the field.
        /// </summary>
        [JsonProperty("t")]
        public ITypeReference Type { get; set; }

        /// <summary>
        /// The constant value of the field, if any.
        /// </summary>
        [JsonProperty("v")]
        public ILiteral Value { get; set; }
    }
}
