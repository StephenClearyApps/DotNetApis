using Newtonsoft.Json;

namespace DotNetApis.Structure.TypeReferences
{
    /// <summary>
    /// A dimension of an array.
    /// </summary>
    public sealed class ArrayDimensionJson
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
