using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget
{
    public sealed class NugetPackageDependency
    {
        public NugetPackageDependency(string packageId, NugetVersionRange versionRange)
        {
            PackageId = packageId;
            VersionRange = versionRange;
        }

        public string PackageId { get; }
        public NugetVersionRange VersionRange { get; }
    }
}
