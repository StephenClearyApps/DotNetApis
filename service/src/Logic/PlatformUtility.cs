using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
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

        public static int NuGetFrameworkOrdering(string framework)
        {
            var name = Prefix(framework);
            var nameIndex = SupportedFrameworks.IndexOf(name);
            return nameIndex == -1 ? int.MaxValue : nameIndex;
        }

        public static string Prefix(string framework)
        {
            var prefixOffset = framework.IndexOfAny(Numbers);
            return prefixOffset == -1 ? framework : framework.Substring(0, prefixOffset);
        }
    }
}
