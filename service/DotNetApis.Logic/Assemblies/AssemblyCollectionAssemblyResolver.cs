using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Cecil;
using DotNetApis.Common;
using Microsoft.Extensions.Logging;
using Mono.Cecil;

namespace DotNetApis.Logic.Assemblies
{
    /// <summary>
    /// A Cecil AssemblyResolver that resolves references using our AssemblyCollection (loading directly from NuGet packages and/or reference assemblies).
    /// </summary>
    public sealed class AssemblyCollectionAssemblyResolver : AssemblyResolverBase
    {
        private readonly ILogger<AssemblyCollectionAssemblyResolver> _logger;
        private readonly AssemblyCollection _collection;
        private readonly Lazy<IAssemblyResolver> _defaultAssemblyResolver = new Lazy<IAssemblyResolver>(() => new DefaultAssemblyResolver());

        public AssemblyCollectionAssemblyResolver(ILoggerFactory loggerFactory, AssemblyCollection collection)
        {
            _logger = loggerFactory.CreateLogger<AssemblyCollectionAssemblyResolver>();
            _collection = collection;
        }

        public override void Dispose()
        {
            if (_defaultAssemblyResolver.IsValueCreated)
                _defaultAssemblyResolver.Value.Dispose();
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

            // F# libraries have an implicit dependency on FSharp.Core.
            if ("FSharp.Core".Equals(name.Name, StringComparison.InvariantCultureIgnoreCase))
                throw new NeedsFSharpCoreException();

            // As a last-ditch resort, check the GAC on whatever machine we're on.
            _logger.FallingBackToGac(name.FullName);
            try
            {
                return _defaultAssemblyResolver.Value.Resolve(name, parameters);
            }
            catch
            {
                _logger.AssemblyResolutionFailed(name.FullName);
                return null;
            }
        }
    }

	internal static partial class Logging
	{
		public static void FallingBackToGac(this ILogger<AssemblyCollectionAssemblyResolver> logger, string fullName) =>
			Logger.Log(logger, 1, LogLevel.Warning, "Unable to resolve assembly {fullName}; falling back on GAC as a last-ditch effort", fullName, null);

		public static void AssemblyResolutionFailed(this ILogger<AssemblyCollectionAssemblyResolver> logger, string fullName) =>
			Logger.Log(logger, 2, LogLevel.Error, "Unable to resolve assembly {fullName}", fullName, null);
	}
}
