using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetApis.StructuredFormatter
{
    /// <summary>
    /// The kind of entity this structured entry represents.
    /// This must be kept in sync with $FrontEnd/src/constants/entityKinds.js
    /// </summary>
    public enum EntityKind
    {
        Class = 0,
        Interface,
        Struct,
        Enum,
        Delegate,
        Method,
        Property,
        Event,
        Field,
    }
}
