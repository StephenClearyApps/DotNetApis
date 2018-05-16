using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using DotNetApis.Common;
using Microsoft.Extensions.Logging;
using NuGet;

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
        private readonly ILogger<NugetRepository> _logger;

        private readonly IPackageRepository _repository = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");

        public NugetRepository(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<NugetRepository>();
        }

        public NugetPackageIdVersion TryLookupLatestPackageVersion(string packageId)
        {
            _logger.LookingUpLatestVersion(packageId);
            var stopwatch = Stopwatch.StartNew();
            var package = _repository.FindPackage(packageId, version: null, allowPrereleaseVersions: false, allowUnlisted: false);
            if (package == null)
            {
                _logger.LookingUpLatestPrereleaseVersion(packageId);
                package = _repository.FindPackage(packageId, version: null, allowPrereleaseVersions: true, allowUnlisted: false);
            }
            if (package == null)
            {
                _logger.NoVersionFound(packageId, stopwatch.Elapsed);
                return null;
            }
            var idver = new NugetPackageIdVersion(packageId, new NugetVersion(package.Version));
            _logger.FoundLatestVersion(idver, packageId, stopwatch.Elapsed);
            return idver;
        }

        public IReadOnlyList<NugetVersion> GetPackageVersions(string packageId)
        {
            _logger.LookingUpAllVersions(packageId);
            var stopwatch = Stopwatch.StartNew();
            var results = _repository.FindPackages(packageId, versionSpec: null, allowPrereleaseVersions: true, allowUnlisted: false).ToList();
            if (results.Count == 0)
            {
                _logger.NoVersionFound(packageId, stopwatch.Elapsed);
                return new NugetVersion[0];
            }
            var result = results.Select(x => new NugetVersion(x.Version)).ToList();
            _logger.FoundVersions(result, packageId, stopwatch.Elapsed);
            return result;
        }

        public NugetPackageIdVersion TryLookupPackage(string packageId, NugetVersionRange versionRange)
        {
            _logger.LookingUpVersionRange(packageId, versionRange);
            var stopwatch = Stopwatch.StartNew();
            var package = _repository.FindPackage(packageId, versionRange.ToVersionSpec(), allowPrereleaseVersions: true, allowUnlisted: true);
            if (package == null)
            {
                _logger.VersionRangeNotFound(packageId, versionRange, stopwatch.Elapsed);
                return null;
            }
            var idver = new NugetPackageIdVersion(packageId, NugetVersion.FromSemanticVersion(package.Version));
            _logger.VersionRangeResolved(packageId, versionRange, idver, stopwatch.Elapsed);
            return idver;
        }

        public NugetFullPackage DownloadPackage(NugetPackageIdVersion idver)
        {
            _logger.DownloadingPackage(idver);
            var stopwatch = Stopwatch.StartNew();
            var package = _repository.FindPackage(idver.PackageId, idver.Version.ToSemanticVersion(), allowPrereleaseVersions: true, allowUnlisted: true);
            if (package == null)
            {
                _logger.PackageNotFound(idver, stopwatch.Elapsed);
                throw new ExpectedException(HttpStatusCode.NotFound, $"Could not find package {idver} on Nuget; this error can happen if NuGet is currently indexing this package; if this is a newly released version, try again in 5 minutes or so.");
            }
            var published = package.Published;
            if (published == null)
            {
                _logger.NoPublishedMetadata(idver, stopwatch.Elapsed);
                throw new InvalidDataException($"Package {idver} from Nuget does not have Published metadata");
            }

            var result = new NugetFullPackage(new DotNetApis.Nuget.NugetPackage(package.GetStream()), new NugetPackageExternalMetadata(published.Value));
            _logger.PackageDownloaded(idver, result, stopwatch.Elapsed);
            return result;
        }
    }

    internal static partial class Logging
    {
        public static void LookingUpLatestVersion(this ILogger<NugetRepository> logger, string packageId) =>
            Logger.Log(logger, 1, LogLevel.Debug, "Looking up latest package version for {packageId}", packageId, null);

        public static void LookingUpLatestPrereleaseVersion(this ILogger<NugetRepository> logger, string packageId) =>
            Logger.Log(logger, 2, LogLevel.Information, "No non-prerelease package version found for {packageId}; looking up latest prerelease package version", packageId, null);

        public static void NoVersionFound(this ILogger<NugetRepository> logger, string packageId, TimeSpan elapsed) =>
            Logger.Log(logger, 3, LogLevel.Warning, "No package version found for {packageId} in {elapsed}", packageId, elapsed, null);

        public static void FoundLatestVersion(this ILogger<NugetRepository> logger, NugetPackageIdVersion idver, string packageId, TimeSpan elapsed) =>
            Logger.Log(logger, 4, LogLevel.Information, "Found version {idver} for {packageId} in {elapsed}", idver, packageId, elapsed, null);

        public static void LookingUpAllVersions(this ILogger<NugetRepository> logger, string packageId) =>
            Logger.Log(logger, 5, LogLevel.Debug, "Lookup up all package versions for {packageId}", packageId, null);

        public static void FoundVersions(this ILogger<NugetRepository> logger, IReadOnlyList<NugetVersion> versions, string packageId, TimeSpan elapsed) =>
            Logger.Log(logger, 6, LogLevel.Information, "Found versions {versions} for {packageId} in {elapsed}", versions.Dumpable(), packageId, elapsed, null);

        public static void LookingUpVersionRange(this ILogger<NugetRepository> logger, string packageId, NugetVersionRange versionRange) =>
            Logger.Log(logger, 7, LogLevel.Debug, "Searching for package {packageId} {versionRange}", packageId, versionRange, null);

        public static void VersionRangeNotFound(this ILogger<NugetRepository> logger, string packageId, NugetVersionRange versionRange, TimeSpan elapsed) =>
            Logger.Log(logger, 8, LogLevel.Warning, "Package {packageId} {versionRange} was not found in {elapsed}", packageId, versionRange, elapsed, null);

        public static void VersionRangeResolved(this ILogger<NugetRepository> logger, string packageId, NugetVersionRange versionRange, NugetPackageIdVersion idver, TimeSpan elapsed) =>
            Logger.Log(logger, 9, LogLevel.Information, "Package {packageId} {versionRange} resolved to {idver} in {elapsed}", packageId, versionRange, idver, elapsed, null);

        public static void DownloadingPackage(this ILogger<NugetRepository> logger, NugetPackageIdVersion idver) =>
            Logger.Log(logger, 10, LogLevel.Debug, "Downloading package {idver} from Nuget", idver, null);

        public static void PackageNotFound(this ILogger<NugetRepository> logger, NugetPackageIdVersion idver, TimeSpan elapsed) =>
            Logger.Log(logger, 11, LogLevel.Error, "Could not find package {idver} on Nuget in {elapsed}", idver, elapsed, null);

        public static void NoPublishedMetadata(this ILogger<NugetRepository> logger, NugetPackageIdVersion idver, TimeSpan elapsed) =>
            Logger.Log(logger, 12, LogLevel.Error, "Package {idver} from Nuget does not have Published metadata in {elapsed}", idver, elapsed, null);

        public static void PackageDownloaded(this ILogger<NugetRepository> logger, NugetPackageIdVersion idver, NugetFullPackage result, TimeSpan elapsed) =>
            Logger.Log(logger, 13, LogLevel.Debug, "Successfully downloaded package {idver} as {result} from Nuget in {elapsed}", idver, result, elapsed, null);
    }
}
