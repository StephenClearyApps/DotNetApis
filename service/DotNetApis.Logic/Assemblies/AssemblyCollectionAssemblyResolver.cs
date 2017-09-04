using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Cecil;
using Microsoft.Extensions.Logging;
using Mono.Cecil;

namespace DotNetApis.Logic.Assemblies
{
    /// <summary>
    /// A Cecil AssemblyResolver that resolves references using our AssemblyCollection (loading directly from NuGet packages and/or reference assemblies).
    /// </summary>
    public sealed class AssemblyCollectionAssemblyResolver : AssemblyResolverBase
    {
        private readonly ILogger _logger;
        private readonly AssemblyCollection _collection;
        private readonly Lazy<IAssemblyResolver> _defaultAssemblyResolver = new Lazy<IAssemblyResolver>(() => new DefaultAssemblyResolver());

        public AssemblyCollectionAssemblyResolver(ILogger logger, AssemblyCollection collection)
        {
            _logger = logger;
            _collection = collection;
        }

        public override AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            parameters = parameters ?? new ReaderParameters();
            if (parameters.AssemblyResolver == null)
                parameters.AssemblyResolver = this;

            // Search for the assembly in the current package, then all dependent packages, and finally all target reference dlls.
            // The assembly is loaded on-demand here.
            var assembly = _collection.AllAssemblies.FirstOrDefault(x => x.Name.Equals(name.Name, StringComparison.InvariantCultureIgnoreCase) && x.AssemblyDefinition != null);
            if (assembly != null)
                return assembly.AssemblyDefinition;

            // As a last-ditch resort, check the GAC on whatever machine we're on.
            _logger.LogWarning("Unable to resolve assembly {name}; falling back on GAC as a last-ditch effort", name.FullName);
            try
            {
                return _defaultAssemblyResolver.Value.Resolve(name, parameters);
            }
            catch
            {
                _logger.LogError("Unable to resolve assembly {name}", name.FullName);
                return null;
            }
        }
    }
}
