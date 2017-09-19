using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Entities
{
    /// <summary>
    /// An entity that can exist at the top level (defined in a namespace, not in another entity).
    /// </summary>
    public interface IHaveNamespace
    {
        /// <summary>
        /// The namespace where this entity exists. If this entity is nested within another entity, then this is <c>null</c>.
        /// </summary>
        [JsonProperty("s")]
        string Namespace { get; set; }
    }
}
