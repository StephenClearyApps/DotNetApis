using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetApis.Structure
{
    public sealed class ArrayLiteral : ILiteral
    {
        public EntityLiteralKind Kind => EntityLiteralKind.Array;

        // TODO: TypeReference

        public IReadOnlyList<ILiteral> Values { get; set; }
    }
}
