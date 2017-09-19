using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetApis.Structure
{
    /// <summary>
    /// Structured documentation representation of the xmldocs for an entity.
    /// </summary>
    public sealed class StructuredXmldoc
    {
        /// <summary>
        /// The <c>summary</c> or <c>value</c> documentation.
        /// </summary>
        public StructuredXmldocNode Basic { get; set; }

        /// <summary>
        /// The <c>remarks</c> documentation.
        /// </summary>
        public StructuredXmldocNode Remarks { get; set; }

        /// <summary>
        /// The <c>example</c> documentation.
        /// </summary>
        public IReadOnlyList<StructuredXmldocNode> Examples { get; set; }

        /// <summary>
        /// The <c>seealso</c> documentation.
        /// </summary>
        public IReadOnlyList<StructuredXmldocNode> SeeAlso { get; set; }

        /// <summary>
        /// The <c>exception</c> documentation.
        /// </summary>
        public IReadOnlyList<StructuredXmldocNode> Exceptions { get; set; }

        /// <summary>
        /// The <c>returns</c> documentation.
        /// </summary>
        public StructuredXmldocNode Returns { get; set; }
    }
}
