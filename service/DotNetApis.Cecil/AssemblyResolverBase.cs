using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace DotNetApis.Cecil
{
    public abstract class AssemblyResolverBase : IAssemblyResolver
    {
        public AssemblyDefinition Resolve(string fullName) => Resolve(AssemblyNameReference.Parse(fullName), new ReaderParameters());

        public AssemblyDefinition Resolve(AssemblyNameReference name) => Resolve(name, new ReaderParameters());

        public AssemblyDefinition Resolve(string fullName, ReaderParameters parameters) => Resolve(AssemblyNameReference.Parse(fullName), parameters);

        public abstract AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters);
    }
}
