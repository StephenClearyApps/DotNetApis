using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetApis.Structure
{
    public sealed class ArrayLiteral : LiteralBase
    {
        public override EntityLiteralKind Kind => EntityLiteralKind.Array;

        // TODO: TypeReference

        public IReadOnlyList<LiteralBase> Values { get; set; }
    }
}
