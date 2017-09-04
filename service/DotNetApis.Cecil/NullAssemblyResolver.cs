using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace DotNetApis.Cecil
{
    /// <summary>
    /// An assembly resolver that never resolves assemblies.
    /// </summary>
    public sealed class NullAssemblyResolver : AssemblyResolverBase
    {
        public override AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters) => null;
    }
}
