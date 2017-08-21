using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NuGet;
using ILogger = Common.ILogger;

namespace Nuget
{
    /// <summary>
    /// Provides methods used to call the NuGet API.
    /// </summary>
    public sealed class NugetRepository
    {
        private readonly ILogger _logger;

        private readonly IPackageRepository _repository = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");

        public NugetRepository(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Attempts to find the latest package version, preferring released versions over unreleased. Only listed versions are considered.
        /// </summary>
        /// <param name="id">The package id.</param>
        public NugetPackageIdVersion TryLookupLatestPackageVersion(string id)
        {
            _logger.Trace($"Looking up latest package version for `{id}`");
            var package = _repository.FindPackage(id, version: null, allowPrereleaseVersions: false, allowUnlisted: false);
            if (package == null)
            {
                _logger.Trace($"No non-prerelease package version found for `{id}`; looking up latest prerelease package version");
                package = _repository.FindPackage(id, version: null, allowPrereleaseVersions: true, allowUnlisted: false);
            }
            if (package == null)
            {
                _logger.Trace($"No package version found for `{id}`");
                return null;
            }
            var result = new NugetPackageIdVersion(id, new NugetVersion(package.Version));
            _logger.Trace($"Found version `{result}` for `{id}`");
            return result;
        }
    }
}
