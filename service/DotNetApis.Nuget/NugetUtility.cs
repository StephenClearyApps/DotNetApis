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
    }
}
