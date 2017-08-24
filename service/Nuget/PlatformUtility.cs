using System.Collections.Generic;

namespace Nuget
{
    public static class PlatformUtility
    {
        // TODO: determine how to handle .NET core libraries and universal windows apps.
        private static readonly List<string> SupportedFrameworks = new List<string>
        {
            "netstandard",
            "net",
            "win",
            "wpa",
            "wp",
            "sl",
        };

        private static readonly char[] Numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        public static bool IsSupported(this PlatformTarget target) => target.NuGetFrameworkOrdering() != int.MaxValue;

        public static int NuGetFrameworkOrdering(this PlatformTarget target)
        {
            var name = target.Prefix();
            var nameIndex = SupportedFrameworks.IndexOf(name);
            return nameIndex == -1 ? int.MaxValue : nameIndex;
        }

        public static string Prefix(this PlatformTarget target)
        {
            var framework = target.ToString();
            var prefixOffset = framework.IndexOfAny(Numbers);
            return prefixOffset == -1 ? framework : framework.Substring(0, prefixOffset);
        }
    }
}
