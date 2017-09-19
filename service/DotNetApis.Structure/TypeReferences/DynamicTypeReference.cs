using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetApis.Structure
{
    /// <summary>
    /// A dynamic reference; that is, an <c>object</c> that is treated by the compiler as <c>dynamic</c>.
    /// </summary>
    public sealed class DynamicTypeReference : ITypeReference
    {
        public EntityReferenceKind Kind => EntityReferenceKind.Dynamic;
    }
}
