using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Nuget;
using DotNetApis.Structure;
using Microsoft.Extensions.Logging;
using Mono.Cecil;

namespace DotNetApis.Logic.Assemblies
{
    /// <summary>
    /// A collection of all assemblies in our current NuGet package, all of its NuGet dependencies, and the platform it is targeting.
    /// </summary>
    public sealed class AssemblyCollection
    {
        private readonly ILogger _logger;
        private readonly NugetPackage _currentPackage;
        private readonly ReaderParameters _readerParameters;
        private readonly List<CurrentPackageAssembly> _currentPackageAssemblies = new List<CurrentPackageAssembly>();
        private readonly List<DependencyPackageAssembly> _dependencyPackageAssemblies = new List<DependencyPackageAssembly>();
        private readonly List<ReferenceAssembly> _referenceAssemblies = new List<ReferenceAssembly>();

        /// <summary>
        /// The combined xmldoc -> dnaid lookup.
        /// </summary>
        private readonly Dictionary<string, string> _xmldocIdToDnaId = new Dictionary<string, string>();

        /// <summary>
        /// The dnaid -> (location, name) lookup cache.
        /// </summary>
        private readonly Dictionary<string, (ILocation, FriendlyName)?> _dnaidLookupCache = new Dictionary<string, (ILocation, FriendlyName)?>();

        public AssemblyCollection(ILogger logger, NugetPackage currentPackage)
        {
            _logger = logger;
            _currentPackage = currentPackage;
            _readerParameters = new ReaderParameters
            {
                AssemblyResolver = new AssemblyCollectionAssemblyResolver(logger, this),
            };
        }

        /// <summary>
        /// Assemblies in the NuGet package we are generating documentation for.
        /// </summary>
        public IReadOnlyList<CurrentPackageAssembly> CurrentPackageAssemblies => _currentPackageAssemblies;

        /// <summary>
        /// Assemblies in NuGet packages that are (direct or indirect) dependencies of the NuGet package we are generating documentation for.
        /// </summary>
        public IReadOnlyList<DependencyPackageAssembly> DependencyPackageAssemblies => _dependencyPackageAssemblies;

        /// <summary>
        /// Platform reference assemblies for the platform we are generating documentation for.
        /// </summary>
        public IReadOnlyList<ReferenceAssembly> ReferenceAssemblies => _referenceAssemblies;

        /// <summary>
        /// Gets all the assemblies; first the ones in the current NuGet package, then NuGet dependencies, and then platform reference assemblies.
        /// </summary>
        public IEnumerable<IAssembly> AllAssemblies => CurrentPackageAssemblies.Cast<IAssembly>().Concat(DependencyPackageAssemblies).Concat(ReferenceAssemblies);

        /// <summary>
        /// Looks up a dnaid in all assemblies and returns its location and friendly name. Returns <c>null</c> if it's not found, or if its assembly hasn't been loaded yet.
        /// </summary>
        /// <param name="dnaid">The dnaid</param>
        public (ILocation Location, FriendlyName FriendlyName)? TryGetDnaIdLocationAndFriendlyName(string dnaid)
        {
            if (_dnaidLookupCache.ContainsKey(dnaid))
                return _dnaidLookupCache[dnaid];
            var result = AllAssemblies.Select(x => x.TryGetDnaIdLocationAndFriendlyName(dnaid)).FirstOrDefault(x => x != null);
            _dnaidLookupCache[dnaid] = result;
            return result;
        }

        /// <summary>
        /// Adds an assembly in the current NuGet package to this collection. The assembly will be loaded on-demand.
        /// </summary>
        /// <param name="path">The path in the NuGet package of the assembly.</param>
        public void AddCurrentPackageAssembly(string path) => _currentPackageAssemblies.Add(new CurrentPackageAssembly(_logger, path, _readerParameters, _xmldocIdToDnaId, _currentPackage));

        /// <summary>
        /// Adds an assembly in a dependent NuGet package to this collection. The assembly will be loaded on-demand.
        /// </summary>
        /// <param name="package">The dependent NuGet package.</param>
        /// <param name="path">The path in that NuGet package of the assembly.</param>
        public void AddDependencyPackageAssembly(NugetPackage package, string path) =>
            _dependencyPackageAssemblies.Add(new DependencyPackageAssembly(_logger, path, _readerParameters, _xmldocIdToDnaId, package));

        /// <summary>
        /// Adds a reference (platform) assembly to this collection. The assembly will be loaded on-demand.
        /// </summary>
        /// <param name="path">The path of the assembly. This does not have to be an on-disk path.</param>
        /// <param name="read">A function that reads the assembly as a stream. Must not be <c>null</c>.</param>
        public void AddReferenceAssembly(string path, Func<Stream> read) => _referenceAssemblies.Add(new ReferenceAssembly(_logger, path, _readerParameters, _xmldocIdToDnaId, read));
    }
}
