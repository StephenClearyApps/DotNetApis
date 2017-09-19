using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Structure.TypeReferences;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Entities
{
    public sealed class DelegateEntity : IEntity, IHaveNamespace, IHaveGenericParameters
    {
        public EntityKind Kind => EntityKind.Delegate;
        public string DnaId { get; set; }
        public string Name { get; set; }
        public IReadOnlyList<AttributeJson> Attributes { get; set; }
        public EntityAccessibility Accessibility { get; set; }
        EntityModifiers IEntity.Modifiers { get; set; } // not used by delegates
        public StructuredXmldoc Xmldoc { get; set; }
        public string Namespace { get; set; }
        public IReadOnlyList<GenericParameter> GenericParameters { get; set; }

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
    }
}
