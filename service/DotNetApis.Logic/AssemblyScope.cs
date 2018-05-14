using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DotNetApis.Common;

namespace DotNetApis.Logic
{
    public sealed class AssemblyScope : ScopeBase<AssemblyScope>
    {
        private AssemblyScope(XContainer xmldoc)
        {
	        Xmldoc = new XmldocIndex(xmldoc);
        }

        public XmldocIndex Xmldoc { get; }

        public static IDisposable Create(XContainer xmldoc) => Create(new AssemblyScope(xmldoc));
    }
}
