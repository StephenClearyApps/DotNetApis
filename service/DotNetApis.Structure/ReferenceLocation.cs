using Newtonsoft.Json;

namespace DotNetApis.Structure
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
