using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Entities
{
    /// <summary>
    /// Structured documentation for a property getter or setter.
    /// </summary>
    public sealed class PropertyMethod
    {
        /// <summary>
        /// The attributes on this method.
        /// </summary>
        [JsonProperty("b")]
        public IReadOnlyList<AttributeJson> Attributes { get; set; }

        /// <summary>
        /// The accessibility, if the accessibility is "more" restrictive than the property accessibility.
        /// </summary>
        [JsonProperty("a")]
        public EntityAccessibility Accessibility { get; set; }
    }
}
