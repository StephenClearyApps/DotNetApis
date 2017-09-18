using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetApis.Structure
{
    /// <summary>
    /// Structured documentation for an attribute.
    /// </summary>
    public sealed class AttributeJson
    {
        /// <summary>
        /// The target of the attribute, e.g., "assembly" or "return". Usually <c>null</c> to indicate the default target.
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// The short name of the attribute, without the namespace and without the "Attribute" suffix.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Location of the attribute type.
        /// </summary>
        public ILocation Location { get; set; }

        // TODO
    }
}
