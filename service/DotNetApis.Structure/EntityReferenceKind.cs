using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetApis.StructuredFormatter
{
    /// <summary>
    /// This must be kept in sync with constants/entityReferenceKinds.js
    /// </summary>
    public enum EntityReferenceKind
    {
        Type = 0,
        Keyword,
        GenericInstance,
        Dynamic,
        GenericParameter,
        Array,
        Reqmod,
        Pointer,
    }
}
