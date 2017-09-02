using DotNetApis.Common;
using Nito.Comparers;
using NuGet.Versioning;

namespace DotNetApis.Nuget
{
    public sealed class NugetVersionRange : EquatableBaseWithOperators<NugetVersionRange>
    {
        private readonly VersionRange _versionRange;

        static NugetVersionRange()
        {
            DefaultComparer = EqualityComparerBuilder.For<NugetVersionRange>()
                .EquateBy(x => x._versionRange, new VersionRangeComparer());
        }

        public NugetVersionRange(VersionRange versionRange)
        {
            _versionRange = versionRange;
        }

        public NugetVersionRange(NuGet.IVersionSpec versionSpec)
        {
            _versionRange = new VersionRange(NugetVersion.FromSemanticVersion(versionSpec.MinVersion).ToNuGetVersion(), versionSpec.IsMinInclusive,
                NugetVersion.FromSemanticVersion(versionSpec.MaxVersion).ToNuGetVersion(), versionSpec.IsMaxInclusive);
        }

        public VersionRange ToVersionRange() => _versionRange;

        public NuGet.IVersionSpec ToVersionSpec()
        {
            return new NuGet.VersionSpec
            {
                IsMaxInclusive = _versionRange.IsMaxInclusive,
                IsMinInclusive = _versionRange.IsMinInclusive,
                MaxVersion = _versionRange.MaxVersion == null ? null : NugetVersion.FromNuGetVersion(_versionRange.MaxVersion).ToSemanticVersion(),
                MinVersion = _versionRange.MinVersion == null ? null : NugetVersion.FromNuGetVersion(_versionRange.MinVersion).ToSemanticVersion()
            };
        }

        public static NugetVersionRange TryMerge(NugetVersionRange a, NugetVersionRange b)
        {
            var result = VersionRange.Combine(Enumerables.Return(a._versionRange, b._versionRange));
            return Equals(result, VersionRange.None) ? null : new NugetVersionRange(result);
        }

        /// <summary>
        /// Returns a string with the formatted version spec.
        /// </summary>
        public override string ToString() => _versionRange.ToNormalizedString();
    }
}
