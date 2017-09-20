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
    /// Structured documentation for a method.
    /// </summary>
    public sealed class MethodEntity : IEntity, IHaveGenericParameters, IHaveExplicitInterface
    {
        public EntityKind Kind => EntityKind.Method;
        public string DnaId { get; set; }
        public string Name { get; set; }
        public IReadOnlyList<AttributeJson> Attributes { get; set; }
        public EntityAccessibility Accessibility { get; set; }
        public EntityModifiers Modifiers { get; set; }
        public Xmldoc.Xmldoc Xmldoc { get; set; }
        public IReadOnlyList<GenericParameterJson> GenericParameters { get; set; }
        public ITypeReference ExplicitInterfaceDeclaringType { get; set; }

        /// <summary>
        /// Method styles.
        /// </summary>
        [JsonProperty("s")]
        public MethodStyles Styles { get; set; }

        /// <summary>
        /// Return type of the method.
        /// </summary>
        [JsonProperty("r")]
        public ITypeReference ReturnType { get; set; }

        /// <summary>
        /// The method parameters.
        /// </summary>
        [JsonProperty("p")]
        public IReadOnlyList<MethodParameter> Parameters { get; set; }
    }
}
