using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Nuget;
using Microsoft.Extensions.Logging;

namespace DotNetApis.Logic
{
    public sealed class NugetPackageDependencyResolver
    {
        private readonly ILogger<NugetPackageDependencyResolver> _logger;
        private readonly PackageDownloader _packageDownloader;
        private readonly PlatformResolver _platformResolver;

        public NugetPackageDependencyResolver(ILoggerFactory loggerFactory, PackageDownloader packageDownloader, PlatformResolver platformResolver)
        {
			_logger = loggerFactory.CreateLogger<NugetPackageDependencyResolver>();
            _packageDownloader = packageDownloader;
            _platformResolver = platformResolver;
        }

        /// <summary>
        /// Walks the transitive dependencies of a specified root package, and returns a collection of all dependencies (not including the root package).
        /// </summary>
        /// <param name="rootPackage">The root package.</param>
        /// <param name="target">The target framework.</param>
        public Task<IReadOnlyCollection<NugetPackage>> ResolveAsync(NugetPackage rootPackage, PlatformTarget target)
        {
            var resolver = new NugetPackageDependencyResolverState(_logger, _packageDownloader, _platformResolver, rootPackage, target);
            return resolver.ProcessAsync();
        }
    }

    /// <summary>
    /// Manages the state necessary when determining transitive dependencies for a NuGet package.
    /// </summary>
    public sealed class NugetPackageDependencyResolverState
    {
        private readonly ILogger<NugetPackageDependencyResolver> _logger;
        private readonly PackageDownloader _packageDownloader;
        private readonly PlatformResolver _platformResolver;
        private readonly PlatformTarget _target;
        private readonly Dictionary<string, NugetPackage> _resolved;
        private readonly Dictionary<string, NugetPackage> _current;
        private readonly Dictionary<string, NugetPackageDependency> _next;

        public NugetPackageDependencyResolverState(ILogger<NugetPackageDependencyResolver> logger, PackageDownloader packageDownloader, PlatformResolver platformResolver, NugetPackage rootPackage, PlatformTarget target)
        {
            _logger = logger;
            _packageDownloader = packageDownloader;
            _platformResolver = platformResolver;
            _target = target;
            _resolved = new Dictionary<string, NugetPackage>(StringComparer.InvariantCultureIgnoreCase);
            _current = new Dictionary<string, NugetPackage>(StringComparer.InvariantCultureIgnoreCase);
            _next = platformResolver.GetCompatiblePackageDependencies(rootPackage, target);
        }

        /// <summary>
        /// Merges current into resolved (clearing out current), then downloads all next packages and places them in current (clearing out next).
        /// </summary>
        /// <returns></returns>
        private async Task MoveNextAsync()
        {
            foreach (var kvp in _current)
                _resolved.Add(kvp.Key, kvp.Value);
            _current.Clear();
            var results = await Task.WhenAll(_next.Select(x => TryDownloadPackageAsync(x.Key, x.Value.VersionRange))).ConfigureAwait(false);
            foreach (var result in results.Where(x => x != null))
                _current.Add(result.Value.PackageId, result.Value.Package);
            _next.Clear();
        }

        /// <summary>
        /// Attemptes to download a NuGet package matching the specified version range. Returns <c>null</c> if not found.
        /// </summary>
        /// <param name="packageId">The package id.</param>
        /// <param name="versionRange">The package version range.</param>
        private async Task<(string PackageId, NugetPackage Package)?> TryDownloadPackageAsync(string packageId, NugetVersionRange versionRange)
        {
            var fullPackage = await _packageDownloader.TryGetPackageAsync(packageId, versionRange).ConfigureAwait(false);
            if (fullPackage == null)
            {
                _logger.LogError("Could not find dependency {packageId}, version {versionRange}", packageId, versionRange);
                return null;
            }
            return (packageId, fullPackage.Package);
        }

        /// <summary>
        /// Executes the state machine and returns its results.
        /// </summary>
        public async Task<IReadOnlyCollection<NugetPackage>> ProcessAsync()
        {
            await MoveNextAsync();
            while (_current.Count != 0)
            {
                foreach (var dependencyEntry in _current.Values.SelectMany(x => _platformResolver.GetCompatiblePackageDependencies(x, _target)))
                {
                    var packageId = dependencyEntry.Key;
                    var dependency = dependencyEntry.Value;
                    if (_resolved.ContainsKey(packageId) || _current.ContainsKey(packageId))
                        continue;
                    if (_next.ContainsKey(packageId))
                    {
                        var merged = NugetVersionRange.TryMerge(dependency.VersionRange, _next[packageId].VersionRange);
                        if (merged == null)
                            _logger.LogWarning("Unable to resolve dependency version conflict for {packageId} between {versionRange1} and {versionRange2}; choosing {chosenVersionRange} for no reason at all", packageId, dependency.VersionRange, _next[packageId].VersionRange, _next[packageId].VersionRange);
                        else
                            _next[packageId] = new NugetPackageDependency(packageId, merged);
                    }
                    else
                    {
                        _next.Add(packageId, dependency);
                    }
                }
                await MoveNextAsync();
            }
            return _resolved.Values.ToArray();
        }
    }
}
