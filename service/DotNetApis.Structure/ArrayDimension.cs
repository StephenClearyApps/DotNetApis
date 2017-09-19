using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.Structure
{
    /// <summary>
    /// A dimension of an array.
    /// </summary>
    public sealed class ArrayDimension
    {
        /// <summary>
        /// The upper bound, if any.
        /// </summary>
        [JsonProperty("u")]
        public int? UpperBound { get; set; }

        /// <summary>
        /// The lower bound, if not zero.
        /// </summary>
        [JsonProperty("l")]
        public int? LowerBound { get; set; }
    }
}
