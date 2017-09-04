using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.StructuredFormatter
{
    /// <summary>
    /// A location in a reference dll.
    /// </summary>
    public sealed class ReferenceLocation
    {
        /// <summary>
        /// The DNA ID of the location.
        /// </summary>
        [JsonProperty("i")]
        public string DnaId { get; set; }
    }
}
