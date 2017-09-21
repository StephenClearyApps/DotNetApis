using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Structure.Util;
using DotNetApis.Structure.Xmldoc;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Entities
{
    /// <summary>
    /// Structured documentation for an enumeration.
    /// </summary>
    public sealed class EnumEntity: IEntity, IHaveNamespace
    {
        public EntityKind Kind => EntityKind.Enum;
        public string DnaId { get; set; }
        public string Name { get; set; }
        public IReadOnlyList<AttributeJson> Attributes { get; set; }
        public EntityAccessibility Accessibility { get; set; }
        EntityModifiers IEntity.Modifiers { get; set; } // Not used by enums.
        public Xmldoc.Xmldoc Xmldoc { get; set; }
        public string Namespace { get; set; }
        
        /// <summary>
        /// The DNA ID of the underlying type, if that type is not <c>System.Int32</c>.
        /// </summary>
        [JsonProperty("u")]
        public string UnderlyingTypeDnaId { get; set; }

        /// <summary>
        /// Whether to prefer hex display of integral values. This is <c>true</c> if this enum has the <c>Flags</c> attribute.
        /// </summary>
        [JsonProperty("h"), JsonConverter(typeof(IntBooleanConverter))]
        public bool PreferHex { get; set; }

        /// <summary>
        /// The fields (defined values of) the enumeration.
        /// </summary>
        [JsonProperty("f")]
        public IReadOnlyList<EnumField> Fields { get; set; }

        public override string ToString() => Namespace == null ? Name : Namespace + "." + Name;
    }
}
