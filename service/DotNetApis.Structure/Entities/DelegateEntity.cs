using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Structure.TypeReferences;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Entities
{
    public sealed class DelegateEntity : IEntity
    {
        public EntityKind Kind => EntityKind.Delegate;
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
        /// Return type of the delegate.
        /// </summary>
        [JsonProperty("r")]
        public ITypeReference ReturnType { get; set; }

        /// <summary>
        /// The delegate parameters.
        /// </summary>
        [JsonProperty("p")]
        public IReadOnlyList<MethodParameter> Parameters { get; set; }

        /// <summary>
        /// Generic parameters of the delegate.
        /// </summary>
        [JsonProperty("g")]
        public IReadOnlyList<GenericParameter> GenericParameters { get; set; }
    }
}
