using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetApis.Structure
{
    /// <summary>
    /// Structured documentation for a type reference.
    /// </summary>
    public abstract class TypeReferenceBase
    {
        /// <summary>
        /// The kind of type reference this is.
        /// </summary>
        public abstract EntityReferenceKind Kind { get; }
    }
}
