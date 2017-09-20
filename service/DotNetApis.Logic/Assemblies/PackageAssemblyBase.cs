using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Nuget;
using Microsoft.Extensions.Logging;
using Mono.Cecil;

namespace DotNetApis.Logic.Assemblies
{
    /// <summary>
    /// An assembly that is read from a NuGet package.
    /// </summary>
    public abstract class PackageAssemblyBase : AssemblyBase
    {
        /// <summary>
        /// Initializes the base type.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="path">The path of the assembly. This can include path segments, the file name, and the extension.</param>
        /// <param name="readerParameters">The parameters used when processing the assembly by Cecil.</param>
        /// <param name="xmldocIdToDnaId">A reference to the shared xmldoc to dnaid mapping, which is updated when the assembly is processed.</param>
        /// <param name="package">The package conaining the assembly.</param>
        protected PackageAssemblyBase(ILogger logger, string path, ReaderParameters readerParameters, IDictionary<string, string> xmldocIdToDnaId, NugetPackage package)
            : base(logger, path, readerParameters, xmldocIdToDnaId)
        {
            Package = package;
            Path = path;
            FileLength = Package.GetFileLength(Path);
        }

        /// <summary>
        /// The package containing this assembly.
        /// </summary>
        protected NugetPackage Package { get; }

        /// <summary>
        /// The path within the package of the assembly.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// The length of the assembly, in bytes.
        /// </summary>
        public long FileLength { get; }

        protected override Stream Read() => Package.ReadFile(Path);
    }
}
