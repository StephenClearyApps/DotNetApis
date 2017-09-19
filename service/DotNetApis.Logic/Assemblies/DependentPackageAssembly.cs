using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Nuget;
using DotNetApis.Structure;
using DotNetApis.Structure.Locations;
using Microsoft.Extensions.Logging;
using Mono.Cecil;

namespace DotNetApis.Logic.Assemblies
{
    /// <summary>
    /// An assembly in a NuGet package that is a (direct or indirect) dependency of the package we're generating documentation for.
    /// </summary>
    public sealed class DependencyPackageAssembly : PackageAssemblyBase
    {
        /// <summary>
        /// Initializes the assembly.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="path">The path of the assembly. This can include path segments, the file name, and the extension.</param>
        /// <param name="readerParameters">The parameters used when processing the assembly by Cecil.</param>
        /// <param name="xmldocIdToDnaId">A reference to the shared xmldoc to dnaid mapping, which is updated when the assembly is processed.</param>
        /// <param name="package">The package conaining the assembly.</param>
        public DependencyPackageAssembly(ILogger logger, string path, ReaderParameters readerParameters, IDictionary<string, string> xmldocIdToDnaId, NugetPackage package)
            : base(logger, path, readerParameters, xmldocIdToDnaId, package)
        {
        }

        protected override ILocation Location(string dnaid) => new DependencyLocation
        {
            PackageId = Package.Metadata.PackageId,
            PackageVersion = Package.Metadata.Version.ToString(),
            DnaId = dnaid,
        };
    }
}
