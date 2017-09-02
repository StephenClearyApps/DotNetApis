using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using NuGet.Frameworks;

namespace DotNetApis.Nuget
{
    /// <summary>
    /// A nupkg that is loaded into memory.
    /// </summary>
    public sealed partial class NugetPackage
    {
        private readonly PackageArchiveReaderWithRef _package;

        /// <summary>
        /// Reads the package and metadata from the in-memory stream.
        /// </summary>
        /// <param name="stream">The in-memory stream. This must be seekable.</param>
        public NugetPackage(Stream stream)
        {
            Stream = stream;
            _package = new PackageArchiveReaderWithRef(stream, leaveStreamOpen: true);
            Metadata = new global::DotNetApis.Nuget.NugetPackage.InternalMetadata(_package);
            Stream.Position = 0;
        }

        /// <summary>
        /// Metadata contained within the package itself.
        /// </summary>
        public global::DotNetApis.Nuget.NugetPackage.InternalMetadata Metadata { get; }

        /// <summary>
        /// Directly access the underlying stream.
        /// </summary>
        public Stream Stream { get; }

        /// <summary>
        /// Gets the length of a file in this package.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        public long GetFileLength(string path) => _package.EnumeratePackageEntries(new[] { path }, "").Single().PackageEntry.Length;

        /// <summary>
        /// Reads a file from this package.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        public Stream ReadFile(string path) => _package.GetStream(path);

        /// <summary>
        /// Gets a list of files for a specific target framework, preferring /ref files over /lib files. Returns an empty enumerable if none are found.
        /// </summary>
        /// <param name="target">The name of the target framework.</param>
        public IEnumerable<string> GetCompatibleAssemblyReferences(FrameworkName target)
        {
            var framework = NuGetFramework.ParseFrameworkName(target.FullName, DefaultFrameworkNameProvider.Instance);
            var result = NuGetFrameworkUtility.GetNearest(_package.GetRefItems(), framework);
            if (result != null)
            {
                var items = result.Items.ToArray();
                if (items.Length != 0)
                    return items;
            }
            result = NuGetFrameworkUtility.GetNearest(_package.GetLibItems(), framework);
            return result == null ? Enumerable.Empty<string>() : result.Items;
        }

        /// <summary>
        /// Gets all supported frameworks.
        /// </summary>
        public IEnumerable<FrameworkName> GetSupportedFrameworks() =>
            _package.GetSupportedFrameworksWithRef().Select(x => new FrameworkName(x.GetDotNetFrameworkName(DefaultFrameworkNameProvider.Instance)));

        /// <summary>
        /// Returns an identifying string for this package, in the form "Id ver".
        /// </summary>
        public override string ToString() => Metadata.ToString();
    }
}
