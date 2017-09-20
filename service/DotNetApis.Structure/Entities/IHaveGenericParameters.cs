using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Entities
{
    /// <summary>
    /// An entity that can be generic.
    /// </summary>
    public interface IHaveGenericParameters
    {
        /// <summary>
        /// Generic parameters of the entity.
        /// </summary>
        [JsonProperty("g")]
        IReadOnlyList<GenericParameterJson> GenericParameters { get; set; }
    }
}
