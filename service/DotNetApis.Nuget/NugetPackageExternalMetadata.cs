using System;

namespace DotNetApis.Nuget
{
    /// <summary>
    /// Package metadata that exists outside the package itself.
    /// </summary>
    public sealed class NugetPackageExternalMetadata
    {
        public NugetPackageExternalMetadata(DateTimeOffset published)
        {
            Published = published;
        }

        /// <summary>
        /// When the package was published.
        /// </summary>
        public DateTimeOffset Published { get; }
    }
}
