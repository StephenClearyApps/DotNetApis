using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetApis.Common
{
    public sealed class NeedsFSharpCoreException : Exception
    {
        public NeedsFSharpCoreException()
            : base("This package requires FSharp.Core")
        {
        }
    }
}
