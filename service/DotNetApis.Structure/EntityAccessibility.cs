using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetApis.StructuredFormatter
{
    /// <summary>
    /// The accessibility of an entity.
    /// This must be kept in sync with constants/entityAccessibility.js
    /// </summary>
    public enum EntityAccessibility
    {
        Public = 0,
        Protected = 1,
        Hidden = 2,
    }
}
