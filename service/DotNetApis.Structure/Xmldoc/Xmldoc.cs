using System.Collections.Generic;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Xmldoc
{
    /// <summary>
    /// Structured documentation representation of the xmldocs for an entity.
    /// </summary>
    public sealed class Xmldoc
    {
        /// <summary>
        /// The <c>summary</c> or <c>value</c> documentation.
        /// </summary>
        [JsonProperty("b")]
        public IXmldocNode Basic { get; set; }

        /// <summary>
        /// The <c>remarks</c> documentation.
        /// </summary>
        [JsonProperty("m")]
        public IXmldocNode Remarks { get; set; }

        /// <summary>
        /// The <c>example</c> documentation.
        /// </summary>
        [JsonProperty("e")]
        public IReadOnlyList<IXmldocNode> Examples { get; set; }

        /// <summary>
        /// The <c>seealso</c> documentation.
        /// </summary>
        [JsonProperty("s")]
        public IReadOnlyList<IXmldocNode> SeeAlso { get; set; }

        /// <summary>
        /// The <c>exception</c> documentation.
        /// </summary>
        [JsonProperty("x")]
        public IReadOnlyList<IXmldocNode> Exceptions { get; set; }

        /// <summary>
        /// The <c>returns</c> documentation.
        /// </summary>
        [JsonProperty("r")]
        public IXmldocNode Returns { get; set; }

        public override string ToString() => Basic?.ToString() ?? "";
    }
}
