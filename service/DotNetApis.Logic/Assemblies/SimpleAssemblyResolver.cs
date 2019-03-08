using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetApis.Cecil;
using Mono.Cecil;

namespace DotNetApis.Logic.Assemblies
{
    public sealed class SimpleAssemblyResolver : AssemblyResolverBase
    {
        private readonly IReadOnlyCollection<AssemblyBase> _referenceAssemblies;

        public SimpleAssemblyResolver(IReadOnlyCollection<AssemblyBase> referenceAssemblies)
        {
            _referenceAssemblies = referenceAssemblies;
        }

        public override AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            // Search for the assembly in the list of assemblies.
            var assembly = _referenceAssemblies.FirstOrDefault(x => x.Name.Equals(name.Name, StringComparison.InvariantCultureIgnoreCase) && x.AssemblyDefinition != null);
            return assembly?.AssemblyDefinition;
        }

        public override void Dispose() { }
    }
}
