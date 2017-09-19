using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Structure.Literals;
using DotNetApis.Structure.TypeReferences;

namespace DotNetApis.Structure.Entities
{
    /// <summary>
    /// A field (or constant value).
    /// </summary>
    public sealed class FieldEntity : IEntity
    {
        public EntityKind Kind => EntityKind.Field;
        public string DnaId { get; set; }

        public IReadOnlyList<AttributeJson> Attributes { get; set; }

        public EntityAccessibility Accessibility { get; set; }

        public string Name { get; set; }

        public EntityModifiers Modifiers { get; set; }

        public ITypeReference Type { get; set; }

        public ILiteral Value { get; set; }

        //public StructuredXmldoc Xmldoc { get; set; }
    }
}
