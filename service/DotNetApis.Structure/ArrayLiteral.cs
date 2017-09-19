using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetApis.Structure
{
    /// <summary>
    /// A constant array.
    /// </summary>
    public sealed class ArrayLiteral : ILiteral
    {
        public EntityLiteralKind Kind => EntityLiteralKind.Array;

        /// <summary>
        /// The type of the array elements.
        /// </summary>
        public ITypeReference ElementType { get; set; }

        /// <summary>
        /// The array element values.
        /// </summary>
        public IReadOnlyList<ILiteral> Values { get; set; }
    }
}
