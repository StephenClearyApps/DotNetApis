using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.Comparers;
using NuGet.Versioning;

namespace Nuget
{
    /// <summary>
    /// Represents the version of a Nuget package. This is an immutable type that is comparable with itself (including comparison operators).
    /// </summary>
    public sealed class NugetVersion : ComparableBaseWithOperators<NugetVersion>
    {
        private readonly NuGetVersion _version;

        static NugetVersion()
        {
            DefaultComparer = ComparerBuilder.For<NugetVersion>().OrderBy(x => x._version, new VersionComparer());
        }

        internal NugetVersion(NuGetVersion version)
        {
            if (version == null)
                throw new InvalidOperationException("version cannot be null");
            _version = version;
        }

        internal NugetVersion(NuGet.SemanticVersion version)
            : this(new NuGetVersion(version.Version, version.SpecialVersion))
        {
        }

        /// <summary>
        /// Gets the major version number.
        /// </summary>
        public int Major => _version.Major;

        /// <summary>
        /// Gets the minor version number.
        /// </summary>
        public int Minor => _version.Minor;

        /// <summary>
        /// Gets the build (third) version number.
        /// </summary>
        public int Build => _version.Patch;

        /// <summary>
        /// Gets the revision (fourth) version number. This version number is not generally used.
        /// </summary>
        public int Revision => _version.Revision;

        /// <summary>
        /// Gets the prerelease string.
        /// </summary>
        public string Prerelease => _version.Release;

        /// <summary>
        /// Whether this package is a prerelease version.
        /// </summary>
        public bool IsPrerelease => Prerelease != string.Empty || Major == 0;

        /// <summary>
        /// Whether this package is a release version (i.e., not prerelease).
        /// </summary>
        public bool IsReleaseVersion => !IsPrerelease;

        /// <summary>
        /// Formats the Nuget version. The major and minor version numbers are always included; other version numbers are only included if non-zero, and the prerelease string is only present if non-empty.
        /// </summary>
        public override string ToString() => _version.ToNormalizedString();

        /// <summary>
        /// Attempts to parse a Nuget version. Returns <c>null</c> if the version number could not be parsed.
        /// </summary>
        /// <param name="version">The version, as a string.</param>
        public static NugetVersion TryParse(string version)
        {
            NuGetVersion result;
            if (!NuGetVersion.TryParse(version, out result))
                return null;
            return new NugetVersion(result);
        }

        /// <summary>
        /// Method to construct a <see cref="SemanticVersion"/> from this Nuget version.
        /// </summary>
        internal NuGet.SemanticVersion ToSemanticVersion() => new NuGet.SemanticVersion(_version.Version, Prerelease);

        /// <summary>
        /// Method to construct a Nuget version from a <see cref="SemanticVersion"/>.
        /// </summary>
        /// <param name="version">The semantic version.</param>
        internal static NugetVersion FromSemanticVersion(NuGet.SemanticVersion version) => new NugetVersion(version);

        internal NuGetVersion ToNuGetVersion() => _version;

        internal static NugetVersion FromNuGetVersion(NuGetVersion version) => new NugetVersion(version);
    }
}
