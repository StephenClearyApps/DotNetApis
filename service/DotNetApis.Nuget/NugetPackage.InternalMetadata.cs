using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
            internal InternalMetadata(IPackageCoreReader package)
            {
                NuspecReader = new NuspecReader(package.GetNuspec());
                PackageId = package.GetIdentity().Id;
                Version = NugetVersion.FromNuGetVersion(package.GetIdentity().Version);
            }

            private static string NullIfEmpty(string data) => data == "" ? null : data;

			/// <summary>
            /// Gets the .nuspec reader for this package.
            /// </summary>
			public NuspecReader NuspecReader { get; }

            public string PackageId { get; }

            public NugetVersion Version { get; }

            public string Title => NullIfEmpty(NuspecReader.GetTitle());

            public string Summary => NullIfEmpty(NuspecReader.GetSummary()) ?? NullIfEmpty(NuspecReader.GetDescription());

            public string Description => NullIfEmpty(NuspecReader.GetDescription()) ?? NullIfEmpty(NuspecReader.GetSummary());

            private static readonly char[] Comma = { ',' };
            public IReadOnlyList<string> Authors
            {
                get
                {
                    var authors = NuspecReader.GetAuthors().Split(Comma, StringSplitOptions.RemoveEmptyEntries);
                    if (authors.Length != 0)
                        return authors.Select(x => x.Trim()).ToList();
                    return NuspecReader.GetMetadata().Where(x => string.Equals(x.Key, "author", StringComparison.InvariantCultureIgnoreCase)).Select(x => x.Value).ToList();
                }
            }

            public string IconUrl => NullIfEmpty(NuspecReader.GetIconUrl());

            public string ProjectUrl => NullIfEmpty(NuspecReader.GetProjectUrl());

            /// <summary>
            /// Returns an identifying string for this package, in the form "Id ver".
            /// </summary>
            public override string ToString() => PackageId + " " + Version;
        }
    }
}
