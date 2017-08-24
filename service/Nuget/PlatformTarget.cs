using System.Linq;
using System.Runtime.Versioning;

namespace Nuget
{
    /// <summary>
    /// A normalized form of <see cref="FrameworkName"/>.
    /// </summary>
    public sealed class PlatformTarget
    {
        private readonly string _shortName;

        public PlatformTarget(FrameworkName framework)
        {
            // Do standard Nuget normalization.
            FrameworkName = framework.NormalizeFrameworkName();

            // Further normalize PCL platforms by ordering their child platforms.
            if (FrameworkName.IsFrameworkPortable())
            {
                var profile = string.Join("+", FrameworkName.Profile.Split('+').OrderBy(x => x));
                if (FrameworkName.Profile != profile)
                    FrameworkName = new FrameworkName(FrameworkName.Identifier, FrameworkName.Version, profile);
            }

            _shortName = FrameworkName.ShortFrameworkName().ToLowerInvariant();
        }

        /// <summary>
        /// Gets the normalized framework name.
        /// </summary>
        public FrameworkName FrameworkName { get; }

        /// <summary>
        /// Returns the standard NuGet name for this target framework, lowercased.
        /// </summary>
        public override string ToString() => _shortName;

        /// <summary>
        /// Attempts to parse a string as a platform target. Returns <c>null</c> if the parsing fails.
        /// </summary>
        /// <param name="targetFramework">The string.</param>
        public static PlatformTarget TryParse(string targetFramework)
        {
            var result = NugetUtility.TryParseFrameworkName(targetFramework);
            return result == null ? null : new PlatformTarget(result);
        }
    }
}
