using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NuGet;

namespace Nuget
{
    /// <summary>
    /// Provides methods used to call the NuGet API.
    /// </summary>
    public sealed class NugetRepository
    {
        private readonly IPackageRepository _repository = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");

        /// <summary>
        /// Attempts to find the latest package version, preferring released versions over unreleased. Only listed versions are considered.
        /// </summary>
        /// <param name="id">The package id.</param>
        public NugetPackageIdVersion TryLookupLatestPackageVersion(string id)
        {
            var package = _repository.FindPackage(id, version: null, allowPrereleaseVersions: false, allowUnlisted: false);
            if (package == null)
                package = _repository.FindPackage(id, version: null, allowPrereleaseVersions: true, allowUnlisted: false);
            if (package == null)
                return null;
            return new NugetPackageIdVersion(id, new NugetVersion(package.Version));
        }
    }
}
