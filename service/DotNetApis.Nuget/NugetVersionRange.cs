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

        public VersionRange ToVersionRange() => _versionRange;

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
