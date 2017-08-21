using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using NuGet.Frameworks;

namespace Nuget
{
    public static class NugetUtility
    {
        /// <summary>
        /// Whether a framework is a PCL framework.
        /// </summary>
        /// <param name="frameworkName">The framework name.</param>
        public static bool IsFrameworkPortable(this FrameworkName frameworkName) => NuGetFramework.ParseFrameworkName(frameworkName.FullName, DefaultFrameworkNameProvider.Instance).IsPCL;

        /// <summary>
        /// Normalizes the framework name into a full framework name.
        /// </summary>
        /// <param name="frameworkName">The framework name.</param>
        public static FrameworkName NormalizeFrameworkName(this FrameworkName frameworkName) => 
            new FrameworkName(NuGetFramework.ParseFrameworkName(frameworkName.FullName, DefaultFrameworkNameProvider.Instance).DotNetFrameworkName);

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
