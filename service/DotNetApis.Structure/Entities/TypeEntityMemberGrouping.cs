using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Entities
{
    /// <summary>
    /// A grouping of entity members.
    /// </summary>
    public sealed class TypeEntityMemberGrouping
    {
        /// <summary>
        /// Members related to lifetime management.
        /// </summary>
        [JsonProperty("l")]
        public IReadOnlyList<IEntity> Lifetime { get; set; }

        /// <summary>
        /// Static members.
        /// </summary>
        [JsonProperty("s")]
        public IReadOnlyList<IEntity> Static { get; set; }

        /// <summary>
        /// Instance members.
        /// </summary>
        [JsonProperty("i")]
        public IReadOnlyList<IEntity> Instance { get; set; }

        /// <summary>
        /// Nested types.
        /// </summary>
        [JsonProperty("t")]
        public IReadOnlyList<IEntity> Types { get; set; }
    }
}
