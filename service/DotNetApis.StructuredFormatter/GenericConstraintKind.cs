using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetApis.StructuredFormatter
{
    /// <summary>
    /// This must be kept in sync with constants/entityGenericConstraintsKinds.js
    /// </summary>
    public enum GenericConstraintKind
    {
        Class = 0,
        Struct,
        New,
        Type,
    }
}
