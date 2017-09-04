using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetApis.StructuredFormatter
{
    /// <summary>
    /// Modifiers on entities.
    /// Must be kept in sync with constants/entityModifiers.js
    /// </summary>
    [Flags]
    public enum EntityModifiers
    {
        None = 0,
        Static = 0x1,
        Abstract = 0x2,
        Sealed = 0x4,
        Virtual = 0x8,
        Override = 0x10,
        Const = 0x20,
    }
}
