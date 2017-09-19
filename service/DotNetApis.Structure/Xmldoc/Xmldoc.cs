using System.Collections.Generic;

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
        public IXmldocNode Basic { get; set; }

        /// <summary>
        /// The <c>remarks</c> documentation.
        /// </summary>
        public IXmldocNode Remarks { get; set; }

        /// <summary>
        /// The <c>example</c> documentation.
        /// </summary>
        public IReadOnlyList<IXmldocNode> Examples { get; set; }

        /// <summary>
        /// The <c>seealso</c> documentation.
        /// </summary>
        public IReadOnlyList<IXmldocNode> SeeAlso { get; set; }

        /// <summary>
        /// The <c>exception</c> documentation.
        /// </summary>
        public IReadOnlyList<IXmldocNode> Exceptions { get; set; }

        /// <summary>
        /// The <c>returns</c> documentation.
        /// </summary>
        public IXmldocNode Returns { get; set; }
    }
}
