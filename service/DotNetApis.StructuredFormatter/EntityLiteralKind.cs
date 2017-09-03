using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetApis.StructuredFormatter
{
    /// <summary>
    /// This must be kept in sync with constants\entityLiteralKinds.js
    /// </summary>
    public enum EntityLiteralKind
    {
        Null = 0,
        Primitive,
        Array,
        Typeof,
        Enum
    }
}
