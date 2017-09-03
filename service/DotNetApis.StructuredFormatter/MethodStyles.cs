using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetApis.StructuredFormatter
{
    /// <summary>
    /// This must be kept in sync with constants/entityMethodStyles.js
    /// </summary>
    public enum MethodStyles
    {
        None = 0,
        Extension = 1,
        Implicit = 2,
        Explicit = 3,
        Operator = 4,
    }
}
