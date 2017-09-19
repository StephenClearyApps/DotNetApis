using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Entities
{
    /// <summary>
    /// An "entity": something with a DNA ID and structured documentation.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// The kind of entity this is.
        /// </summary>
        [JsonProperty("k")]
        EntityKind Kind { get; }

        /// <summary>
        /// The DNA ID of the entity.
        /// </summary>
        [JsonProperty("i")]
        string DnaId { get; set; }
    }
}
