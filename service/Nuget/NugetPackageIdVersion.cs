using System;
using Nito.Comparers;

namespace DotNetApis.Nuget
{
    /// <summary>
    /// A specific version of a package.
    /// </summary>
    public sealed class NugetPackageIdVersion : IEquatable<NugetPackageIdVersion>
    {
        /// <summary>
        /// Creates a new <see cref="NugetPackageIdVersion"/>.
        /// </summary>
        /// <param name="packageId">The package id. This may not be <c>null</c>.</param>
        /// <param name="version">The version. This may not be <c>null</c>.</param>
        public NugetPackageIdVersion(string packageId, NugetVersion version)
        {
            PackageId = packageId?.ToLowerInvariant() ?? throw new ArgumentNullException(nameof(packageId));
            Version = version ?? throw new ArgumentNullException(nameof(version));
        }

        /// <summary>
        /// Attempts to create an <c>IdVersion</c>. If there's no id or no version specified, this method returns <c>null</c>.
        /// </summary>
        /// <param name="packageId">The package id, if known. This may be <c>null</c>, in which case this method returns <c>null</c>.</param>
        /// <param name="version">The package version, if known. This may be <c>null</c>, in which case this method returns <c>null</c>.</param>
        public static NugetPackageIdVersion TryCreate(string packageId, NugetVersion version)
        {
            if (packageId == null || version == null)
                return null;
            return new NugetPackageIdVersion(packageId, version);
        }

        /// <summary>
        /// The id of the package, lowercased. This is never <c>null</c>.
        /// </summary>
        public string PackageId { get; }

        /// <summary>
        /// The version of the package. This is never <c>null</c>.
        /// </summary>
        public NugetVersion Version { get; }

        private static readonly IFullEqualityComparer<NugetPackageIdVersion> DefaultComparer = EqualityComparerBuilder.For<NugetPackageIdVersion>().EquateBy(x => x.PackageId).ThenEquateBy(x => x.Version);

        public bool Equals(NugetPackageIdVersion other) => Nito.Comparers.Util.ComparableImplementations.ImplementEquals(DefaultComparer, this, other);

        public override bool Equals(object obj) => Nito.Comparers.Util.ComparableImplementations.ImplementEquals(DefaultComparer, this, obj);

        public override int GetHashCode() => Nito.Comparers.Util.ComparableImplementations.ImplementGetHashCode(DefaultComparer, this);

        /// <summary>
        /// Returns an identifying string for this package, in the form "id/ver". Note that the id is lowercased.
        /// </summary>
        public override string ToString() => PackageId + "/" + Version;
    }
}
