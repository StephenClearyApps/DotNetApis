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
    /// Structured documentation for a class, struct, or interface.
    /// </summary>
    public sealed class ObjectEntity : IEntity
    {
        public EntityKind Kind { get; set; }
        public string DnaId { get; set; }
        public string Name { get; set; }
        public IReadOnlyList<AttributeJson> Attributes { get; set; }
        public StructuredXmldoc Xmldoc { get; set; }

        public EntityAccessibility Accessibility { get; set; }

        public EntityModifiers Modifiers { get; set; }

        public string Namespace { get; set; }

        [JsonProperty("g")]
        public IReadOnlyList<GenericParameter> GenericParameters { get; set; }

        public IReadOnlyList<ITypeReference> BaseTypesAndInterfaces { get; set; }



        // TODO
    }
}
