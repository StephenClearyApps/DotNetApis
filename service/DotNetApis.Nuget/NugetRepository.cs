using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DotNetApis.Common;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using NuGet.Configuration;
using NuGet.Packaging.Core;
using NuGet.Packaging.Signing;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

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
        Task<NugetPackageIdVersion> TryLookupLatestPackageVersionAsync(string id);

        /// <summary>
        /// Attempts to find a specified package version matching a version range. Returns <c>null</c> if no matching package version is found.
        /// </summary>
        /// <param name="id">The package id.</param>
        /// <param name="versionRange">The version range the package must match.</param>
        /// <returns></returns>
        Task<NugetPackageIdVersion> TryLookupPackageAsync(string id, NugetVersionRange versionRange);

        /// <summary>
        /// Downloads a specific package from Nuget. Throws <see cref="ExpectedException"/> (404) if not found.
        /// </summary>
        /// <param name="idver">The identity of the package.</param>
        Task<NugetFullPackage> DownloadPackageAsync(NugetPackageIdVersion idver);

        /// <summary>
        /// Gets all versions for a package, including prerelease versions.
        /// </summary>
        /// <param name="packageId">The package id.</param>
        Task<IReadOnlyList<NugetVersion>> GetPackageVersionsAsync(string packageId);
    }

    public sealed class NugetRepository : INugetRepository
    {
        private readonly ILogger<NugetRepository> _logger;
        private readonly NuGet.Common.ILogger _nugetLogger;

        private readonly ISettings _nugetSettings;
        private readonly SourceCacheContext _nugetCache;
        private readonly AsyncLazy<MetadataResource> _metadataResource;
        private readonly AsyncLazy<PackageMetadataResource> _packageMetadataResource;
        private readonly AsyncLazy<DownloadResource> _downloadResource;

        public NugetRepository(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<NugetRepository>();
            _nugetLogger = NuGet.Common.NullLogger.Instance;
            _nugetCache = new SourceCacheContext() { DirectDownload = true, NoCache = true };
            _nugetSettings = Settings.LoadDefaultSettings(null);
            var repositories = new SourceRepositoryProvider(_nugetSettings, Repository.Provider.GetCoreV3())
                .GetRepositories();
            _metadataResource = new AsyncLazy<MetadataResource>(() => FindResource<MetadataResource>(repositories));
            _packageMetadataResource = new AsyncLazy<PackageMetadataResource>(() => FindResource<PackageMetadataResource>(repositories));
            _downloadResource = new AsyncLazy<DownloadResource>(() => FindResource<DownloadResource>(repositories));
        }

        private async Task<T> FindResource<T>(IEnumerable<SourceRepository> repositories) where T : class, INuGetResource
        {
            foreach (var repository in repositories)
            {
                var result = await repository.GetResourceAsync<T>();
                if (result != null)
                    return result;
            }

            return null;
        }

        public async Task<NugetPackageIdVersion> TryLookupLatestPackageVersionAsync(string packageId)
        {
            _logger.LookingUpLatestVersion(packageId);
            var stopwatch = Stopwatch.StartNew();

            var resource = await _metadataResource;
            var version = await resource.GetLatestVersion(
                packageId,
                includePrerelease: false,
                includeUnlisted: false,
                sourceCacheContext: _nugetCache,
                log: _nugetLogger,
                token: CancellationToken.None);
            if (version == null)
            {
                _logger.LookingUpLatestPrereleaseVersion(packageId);
                version = await resource.GetLatestVersion(
                    packageId,
                    includePrerelease: true,
                    includeUnlisted: false,
                    sourceCacheContext: _nugetCache,
                    log: _nugetLogger,
                    token: CancellationToken.None);
            }
            if (version == null)
            {
                _logger.NoVersionFound(packageId, stopwatch.Elapsed);
                return null;
            }
            var idver = new NugetPackageIdVersion(packageId, new NugetVersion(version));
            _logger.FoundLatestVersion(idver, packageId, stopwatch.Elapsed);
            return idver;
        }

        public async Task<IReadOnlyList<NugetVersion>> GetPackageVersionsAsync(string packageId)
        {
            _logger.LookingUpAllVersions(packageId);
            var stopwatch = Stopwatch.StartNew();
            var resource = await _metadataResource;
            var versions = await resource.GetVersions(
                packageId,
                includePrerelease: true,
                includeUnlisted: false,
                sourceCacheContext: _nugetCache,
                log: _nugetLogger,
                token: CancellationToken.None);
            var results = versions.ToList();
            if (results.Count == 0)
            {
                _logger.NoVersionFound(packageId, stopwatch.Elapsed);
                return new NugetVersion[0];
            }
            var result = results.Select(x => new NugetVersion(x)).ToList();
            _logger.FoundVersions(result, packageId, stopwatch.Elapsed);
            return result;
        }

        public async Task<NugetPackageIdVersion> TryLookupPackageAsync(string packageId, NugetVersionRange versionRange)
        {
            _logger.LookingUpVersionRange(packageId, versionRange);
            var stopwatch = Stopwatch.StartNew();
            var vr = versionRange.ToVersionRange();
            var resource = await _metadataResource;
            var versions = await resource.GetVersions(
                packageId,
                includePrerelease: true,
                includeUnlisted: true,
                sourceCacheContext: _nugetCache,
                log: _nugetLogger,
                token: CancellationToken.None);
            var all = versions.ToList();
            all.Sort(new VersionComparer());
            var result = all.FirstOrDefault(x => vr.Satisfies(x));
            if (result == null)
            {
                _logger.VersionRangeNotFound(packageId, versionRange, stopwatch.Elapsed);
                return null;
            }
            foreach (var version in all)
                if (vr.IsBetter(result, version))
                    result = version;
            var idver = new NugetPackageIdVersion(packageId, new NugetVersion(result));
            _logger.VersionRangeResolved(packageId, versionRange, idver, stopwatch.Elapsed);
            return idver;
        }

        public async Task<NugetFullPackage> DownloadPackageAsync(NugetPackageIdVersion idver)
        {
            _logger.DownloadingPackage(idver);
            var stopwatch = Stopwatch.StartNew();
            var identity = new PackageIdentity(idver.PackageId, idver.Version.ToNuGetVersion());
            var downloadResource = await _downloadResource;
            var packageMetadataResource = await _packageMetadataResource;
            var metadata = await packageMetadataResource.GetMetadataAsync(identity, _nugetCache, _nugetLogger, CancellationToken.None);
            var packageDownloadContext = new PackageDownloadContext(_nugetCache, _nugetCache.GeneratedTempFolder, directDownload: true)
            {
                ClientPolicyContext = ClientPolicyContext.GetClientPolicy(_nugetSettings, _nugetLogger),
            };
            var downloadResult = await downloadResource.GetDownloadResourceResultAsync(
                identity,
                packageDownloadContext,
                SettingsUtility.GetGlobalPackagesFolder(_nugetSettings),
                _nugetLogger,
                CancellationToken.None);
            if (downloadResult.Status != DownloadResourceResultStatus.Available)
            {
                _logger.PackageNotFound(idver, stopwatch.Elapsed);
                throw new ExpectedException(HttpStatusCode.NotFound, $"Could not find package {idver} on Nuget; this error can happen if NuGet is currently indexing this package; if this is a newly released version, try again in 5 minutes or so.");
            }

            if (metadata.Published == null)
            {
                _logger.NoPublishedMetadata(idver, stopwatch.Elapsed);
                throw new InvalidDataException($"Package {idver} from Nuget does not have Published metadata");
            }

            var result = new NugetFullPackage(new NugetPackage(downloadResult.PackageStream), new NugetPackageExternalMetadata(metadata.Published.Value));
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
