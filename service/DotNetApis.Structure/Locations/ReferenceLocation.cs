using Newtonsoft.Json;

namespace DotNetApis.Structure.Locations
{
    /// <summary>
    /// A location in a reference dll.
    /// </summary>
    public sealed class ReferenceLocation : ILocation
    {
        /// <summary>
        /// The DNA ID of the location.
        /// </summary>
        [JsonProperty("i")]
        public string DnaId { get; set; }
    }
}
