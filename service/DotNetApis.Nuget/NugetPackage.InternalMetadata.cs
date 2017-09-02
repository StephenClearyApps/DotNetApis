using System;
using System.Collections.Generic;
using System.Linq;
using NuGet.Packaging;
using NuGet.Packaging.Core;

namespace DotNetApis.Nuget
{
    partial class NugetPackage
    {
        /// <summary>
        /// Package metadata contained within the package itself.
        /// </summary>
        public sealed class InternalMetadata
        {
            private readonly List<KeyValuePair<string, string>> _metadata;

            internal InternalMetadata(IPackageCoreReader package)
            {
                NuspecReader = new NuspecReader(package.GetNuspec());
                _metadata = NuspecReader.GetMetadata().ToList();
                PackageId = package.GetIdentity().Id;
                Version = NugetVersion.FromNuGetVersion(package.GetIdentity().Version);
            }

            private string ReadMetadata(string key) => _metadata.FirstOrDefault(x => string.Equals(x.Key, key, StringComparison.InvariantCultureIgnoreCase)).Value;

            private IReadOnlyList<string> ReadMultiMetadata(string key) =>
				_metadata.Where(x => string.Equals(x.Key, key, StringComparison.InvariantCultureIgnoreCase)).Select(x => x.Value).ToList();

			/// <summary>
            /// Gets the .nuspec reader for this package.
            /// </summary>
			public NuspecReader NuspecReader { get; }

            public string PackageId { get; }

            public NugetVersion Version { get; }

            public string Title => ReadMetadata("title");

            public string Summary => ReadMetadata("summary") ?? ReadMetadata("description");

            public string Description => ReadMetadata("description") ?? ReadMetadata("summary");

            public IReadOnlyList<string> Authors => ReadMultiMetadata("author");

            public string IconUrl => ReadMetadata("iconurl");

            public string ProjectUrl => ReadMetadata("projecturl");

            /// <summary>
            /// Returns an identifying string for this package, in the form "Id ver".
            /// </summary>
            public override string ToString() => PackageId + " " + Version;
        }
    }
}
