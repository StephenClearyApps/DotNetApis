using System.IO;
using System.Net;
using DotNetApis.Common;
using Microsoft.Extensions.Logging;
using NuGet;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace DotNetApis.Nuget
{
    /// <summary>
    /// Provides methods used to call the NuGet API.
    /// </summary>
    public interface INugetRepository
    {
        /// <summary>
        /// Attempts to find the latest package version, preferring released versions over unreleased. Only listed versions are considered. Returns <c>null</c> if no package is found.
        /// </summary>
        /// <param name="id">The package id.</param>
        NugetPackageIdVersion TryLookupLatestPackageVersion(string id);

        /// <summary>
        /// Attempts to find a specified package version matching a version range. Returns <c>null</c> if no matching package version is found.
        /// </summary>
        /// <param name="id">The package id.</param>
        /// <param name="versionRange">The version range the package must match.</param>
        /// <returns></returns>
        NugetPackageIdVersion TryLookupPackage(string id, NugetVersionRange versionRange);

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

        public NugetPackageIdVersion TryLookupLatestPackageVersion(string packageId)
        {
            _logger.LogDebug("Looking up latest package version for {packageId}", packageId);
            var package = _repository.FindPackage(packageId, version: null, allowPrereleaseVersions: false, allowUnlisted: false);
            if (package == null)
            {
                _logger.LogInformation("No non-prerelease package version found for {packageId}; looking up latest prerelease package version", packageId);
                package = _repository.FindPackage(packageId, version: null, allowPrereleaseVersions: true, allowUnlisted: false);
            }
            if (package == null)
            {
                _logger.LogWarning("No package version found for {packageId}", packageId);
                return null;
            }
            var idver = new NugetPackageIdVersion(packageId, new NugetVersion(package.Version));
            _logger.LogInformation("Found version {idver} for {packageId}", idver, packageId);
            return idver;
        }

        public NugetPackageIdVersion TryLookupPackage(string packageId, NugetVersionRange versionRange)
        {
            _logger.LogDebug("Searching for package matching id {packageId} and version range {versionRange}", packageId, versionRange);
            var package = _repository.FindPackage(packageId, versionRange.ToVersionSpec(), allowPrereleaseVersions: true, allowUnlisted: true);
            if (package == null)
            {
                _logger.LogWarning("Package {packageId} matching version {versionRange} was not found", packageId, versionRange);
                return null;
            }
            var idver = new NugetPackageIdVersion(packageId, NugetVersion.FromSemanticVersion(package.Version));
            _logger.LogInformation("Package {packageId} matching version {versionRange} resolved to {idver}", packageId, versionRange, idver);
            return idver;
        }

        public NugetFullPackage DownloadPackage(NugetPackageIdVersion idver)
        {
            _logger.LogDebug("Downloading package {idver} from Nuget", idver);
            var package = _repository.FindPackage(idver.PackageId, idver.Version.ToSemanticVersion(), allowPrereleaseVersions: true, allowUnlisted: true);
            if (package == null)
            {
                _logger.LogError("Could not find package {idver} on Nuget", idver);
                throw new ExpectedException(HttpStatusCode.NotFound, $"Could not find package {idver} on Nuget; this error can happen if NuGet is currently indexing this package; if this is a newly released version, try again in 5 minutes or so.");
            }
            var published = package.Published;
            if (published == null)
                throw new InvalidDataException($"Package {idver} from Nuget does not have Published metadata");
            var result = new NugetFullPackage(new DotNetApis.Nuget.NugetPackage(package.GetStream()), new NugetPackageExternalMetadata(published.Value));
            _logger.LogDebug("Successfully downloaded package {idver} as {result} from Nuget", idver, result);
            return result;
        }
    }
}
