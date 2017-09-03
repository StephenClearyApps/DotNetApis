using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetApis.StructuredFormatter
{
    /// <summary>
    /// This must be kept in sync with constants/entityMethodParameterModifiers.js
    /// </summary>
    public enum MethodParameterModifiers
    {
        None = 0,
        Params = 1,
        Ref = 2,
        Out = 3,
    }
}
