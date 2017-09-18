using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetApis.Structure
{
    /// <summary>
    /// Structured documentation for an assembly within a NuGet package.
    /// </summary>
    public sealed class AssemblyJson
    {
        public string FullName { get; set; }
        public string Path { get; set; }
        public long FileLength { get; set; }
        // TODO
    }
}
