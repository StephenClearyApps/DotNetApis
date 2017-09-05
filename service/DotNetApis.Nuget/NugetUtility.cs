using System.Linq;
using System.Runtime.Versioning;
using NuGet.Frameworks;

namespace DotNetApis.Nuget
{
    public static class NugetUtility
    {
        /// <summary>
        /// Whether a framework is a PCL framework.
        /// </summary>
        /// <param name="frameworkName">The framework name.</param>
        public static bool IsFrameworkPortable(this FrameworkName frameworkName) => NuGetFramework.ParseFrameworkName(frameworkName.FullName, DefaultFrameworkNameProvider.Instance).IsPCL;

        /// <summary>
        /// Normalizes the framework name, extending it into a full framework name if necessary, and choosing the most desirable equivalent (e.g., `win81` instead of `netcore451`).
        /// </summary>
        /// <param name="frameworkName">The framework name.</param>
        public static FrameworkName NormalizeFrameworkName(this FrameworkName frameworkName)
        {
            var provider = DefaultFrameworkNameProvider.Instance;

            // Noramlize into a full framework name.
            var result = NuGetFramework.ParseFrameworkName(frameworkName.FullName, provider);

            // Choose the most desirable form of the framework name.
            if (provider.TryGetEquivalentFrameworks(result, out var options))
            {
                foreach (var option in options)
                {
                    if (provider.CompareEquivalentFrameworks(result, option) > 0)
                        result = option;
                }
            }

            return new FrameworkName(result.DotNetFrameworkName);
        }

        /// <summary>
        /// Gets the short framework name.
        /// </summary>
        /// <param name="frameworkName">The framework name.</param>
        public static string ShortFrameworkName(this FrameworkName frameworkName) => 
            NuGetFramework.ParseFrameworkName(frameworkName.FullName, DefaultFrameworkNameProvider.Instance).GetShortFolderName();

        /// <summary>
        /// Attempts to parse a string as a framework name.
        /// </summary>
        /// <param name="frameworkString">The string.</param>
        public static FrameworkName TryParseFrameworkName(string frameworkString)
        {
            try
            {
                return new FrameworkName(NuGetFramework.ParseFolder(frameworkString).DotNetFrameworkName);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Tests whether the "platform" framework is sufficiently compatible for a given "library" framework.
        /// E.g., IsCompatible(net45, net40) -> true, IsCompatible(net40, net45) -> false
        /// </summary>
        /// <param name="platformFramework">The "platform" framework.</param>
        /// <param name="libraryFramework">The "library" framework.</param>
        public static bool IsCompatible(FrameworkName platformFramework, FrameworkName libraryFramework)
        {
            var platform = NuGetFramework.ParseFrameworkName(platformFramework.FullName, DefaultFrameworkNameProvider.Instance);
            var library = NuGetFramework.ParseFrameworkName(libraryFramework.FullName, DefaultFrameworkNameProvider.Instance);
            return DefaultCompatibilityProvider.Instance.IsCompatible(platform, library);
        }
    }
}
