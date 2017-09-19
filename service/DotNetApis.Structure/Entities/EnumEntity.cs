using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Structure.Util;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Entities
{
    public sealed class EnumEntity: IEntity
    {
        public EntityKind Kind => EntityKind.Enum;
        public string DnaId { get; set; }
        public string Name { get; set; }
        public IReadOnlyList<AttributeJson> Attributes { get; set; }
        public StructuredXmldoc Xmldoc { get; set; }

        /// <summary>
        /// Accessibility of the entity.
        /// </summary>
        [JsonProperty("a")]
        public EntityAccessibility Accessibility { get; set; }

        [JsonProperty("s")]
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

        [JsonProperty("f")]
        public IReadOnlyList<EnumField> Fields { get; set; }
    }
}
