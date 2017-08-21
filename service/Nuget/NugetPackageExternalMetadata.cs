using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget
{
    /// <summary>
    /// Package metadata that exists outside the package itself.
    /// </summary>
    public sealed class NugetPackageExternalMetadata
    {
        public NugetPackageExternalMetadata(DateTimeOffset published)
        {
            Published = published;
        }

        /// <summary>
        /// When the package was published.
        /// </summary>
        public DateTimeOffset Published { get; }
    }
}
