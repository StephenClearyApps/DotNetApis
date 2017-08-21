using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget
{
    /// <summary>
    /// An in-memory nupkg along with its external metadata.
    /// </summary>
    public sealed class NugetFullPackage
    {
        public NugetFullPackage(NugetPackage package, NugetPackageExternalMetadata externalMetadata)
        {
            Package = package ?? throw new ArgumentNullException(nameof(package));
            ExternalMetadata = externalMetadata ?? throw new ArgumentNullException(nameof(externalMetadata));
        }

        /// <summary>
        /// The in-memory package.
        /// </summary>
        public NugetPackage Package { get; }

        /// <summary>
        /// The external metadata for the package.
        /// </summary>
        public NugetPackageExternalMetadata ExternalMetadata { get; }

        /// <summary>
        /// Returns an identifying string for this package, in the form "Id ver".
        /// </summary>
        public override string ToString() => Package.ToString();
    }
}
