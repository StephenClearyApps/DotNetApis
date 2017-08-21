using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;
using NuGet;
using ILogger = Common.ILogger;

namespace Nuget
{
    /// <summary>
    /// Provides methods used to call the NuGet API.
    /// </summary>
    public interface INugetRepository
    {
        /// <summary>
        /// Attempts to find the latest package version, preferring released versions over unreleased. Only listed versions are considered.
        /// </summary>
        /// <param name="id">The package id.</param>
        NugetPackageIdVersion TryLookupLatestPackageVersion(string id);

        /// <summary>
        /// Downloads a specific package from Nuget. Throws <see cref="ExpectedException"/> (404) if not found.
        /// </summary>
        /// <param name="idver">The identity of the package.</param>
        NugetFullPackage DownloadPackage(NugetPackageIdVersion idver);
    }

    public sealed class NugetRepository : INugetRepository
    {
        private readonly ILogger _logger;

        private readonly IPackageRepository _repository = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");

        public NugetRepository(ILogger logger)
        {
            _logger = logger;
        }

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
            var idver = new NugetPackageIdVersion(id, new NugetVersion(package.Version));
            _logger.Trace($"Found version `{idver}` for `{id}`");
            return idver;
        }

        /// <summary>
        /// Downloads a specific package from Nuget.
        /// </summary>
        /// <param name="idver">The identity of the package.</param>
        public NugetFullPackage DownloadPackage(NugetPackageIdVersion idver)
        {
            _logger.Trace($"Downloading package {idver} from Nuget");
            var package = _repository.FindPackage(idver.PackageId, idver.Version.ToSemanticVersion(), allowPrereleaseVersions: true, allowUnlisted: true);
            if (package == null)
                throw new ExpectedException(HttpStatusCode.NotFound, $"Could not find package {idver}; this error can happen if NuGet is currently indexing this package; if this is a newly released version, try again in 5 minutes or so.");
            var published = package.Published;
            if (published == null)
                throw new InvalidDataException($"Package {idver} from Nuget does not have Published metadata");
            var result = new NugetFullPackage(new NugetPackage(package.GetStream()), new NugetPackageExternalMetadata(published.Value));
            _logger.Trace($"Successfully downloaded package {idver} as `{result}` from Nuget");
            return result;
        }
    }
}
