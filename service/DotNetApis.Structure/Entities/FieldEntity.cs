﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Structure.Literals;
using DotNetApis.Structure.TypeReferences;
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
        public StructuredXmldoc Xmldoc { get; set; }
        public IReadOnlyList<AttributeJson> Attributes { get; set; }

        /// <summary>
        /// The field accessibility.
        /// </summary>
        [JsonProperty("a")]
        public EntityAccessibility Accessibility { get; set; }

        /// <summary>
        /// The field modifiers.
        /// </summary>
        [JsonProperty("m")]
        public EntityModifiers Modifiers { get; set; }

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
