using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        /// <summary>
        /// Gets all versions for a package, including prerelease versions.
        /// </summary>
        /// <param name="packageId">The package id.</param>
        IReadOnlyList<NugetVersion> GetPackageVersions(string packageId);
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
            var stopwatch = Stopwatch.StartNew();
            var package = _repository.FindPackage(packageId, version: null, allowPrereleaseVersions: false, allowUnlisted: false);
            if (package == null)
            {
                _logger.LogInformation("No non-prerelease package version found for {packageId}; looking up latest prerelease package version", packageId);
                package = _repository.FindPackage(packageId, version: null, allowPrereleaseVersions: true, allowUnlisted: false);
            }
            if (package == null)
            {
                _logger.LogWarning("No package version found for {packageId} in {elapsed}", packageId, stopwatch.Elapsed);
                return null;
            }
            var idver = new NugetPackageIdVersion(packageId, new NugetVersion(package.Version));
            _logger.LogInformation("Found version {idver} for {packageId} in {elapsed}", idver, packageId, stopwatch.Elapsed);
            return idver;
        }

        public IReadOnlyList<NugetVersion> GetPackageVersions(string packageId)
        {
            _logger.LogDebug("Lookup up all package versions for {packageId}", packageId);
            var stopwatch = Stopwatch.StartNew();
            var results = _repository.FindPackages(packageId, versionSpec: null, allowPrereleaseVersions: true, allowUnlisted: false).ToList();
            if (results.Count == 0)
            {
                _logger.LogWarning("No package versions found for {packageId} in {elapsed}", packageId, stopwatch.Elapsed);
                return new NugetVersion[0];
            }
            var result = results.Select(x => new NugetVersion(x.Version)).ToList();
            _logger.LogInformation("Found versions {versions} for {packageId} in {elapsed}", result.Dump(), packageId, stopwatch.Elapsed);
            return result;
        }

        public NugetPackageIdVersion TryLookupPackage(string packageId, NugetVersionRange versionRange)
        {
            _logger.LogDebug("Searching for package {packageId} {versionRange}", packageId, versionRange);
            var stopwatch = Stopwatch.StartNew();
            var package = _repository.FindPackage(packageId, versionRange.ToVersionSpec(), allowPrereleaseVersions: true, allowUnlisted: true);
            if (package == null)
            {
                _logger.LogWarning("Package {packageId} {versionRange} was not found in {elapsed}", packageId, versionRange, stopwatch.Elapsed);
                return null;
            }
            var idver = new NugetPackageIdVersion(packageId, NugetVersion.FromSemanticVersion(package.Version));
            _logger.LogInformation("Package {packageId} {versionRange} resolved to {idver} in {elapsed}", packageId, versionRange, idver, stopwatch.Elapsed);
            return idver;
        }

        public NugetFullPackage DownloadPackage(NugetPackageIdVersion idver)
        {
            _logger.LogDebug("Downloading package {idver} from Nuget", idver);
            var stopwatch = Stopwatch.StartNew();
            var package = _repository.FindPackage(idver.PackageId, idver.Version.ToSemanticVersion(), allowPrereleaseVersions: true, allowUnlisted: true);
            if (package == null)
            {
                _logger.LogError("Could not find package {idver} on Nuget in {elapsed}", idver, stopwatch.Elapsed);
                throw new ExpectedException(HttpStatusCode.NotFound, $"Could not find package {idver} on Nuget; this error can happen if NuGet is currently indexing this package; if this is a newly released version, try again in 5 minutes or so.");
            }
            var published = package.Published;
            if (published == null)
                throw new InvalidDataException($"Package {idver} from Nuget does not have Published metadata");
            var result = new NugetFullPackage(new DotNetApis.Nuget.NugetPackage(package.GetStream()), new NugetPackageExternalMetadata(published.Value));
            _logger.LogDebug("Successfully downloaded package {idver} as {result} from Nuget in {elapsed}", idver, result, stopwatch.Elapsed);
            return result;
        }
    }
}
