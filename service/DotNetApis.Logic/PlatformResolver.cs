using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Nuget;
using Microsoft.Extensions.Logging;
using Nito.Comparers;
using NuGet.Frameworks;
using NuGet.Packaging.Core;

namespace DotNetApis.Logic
{
    public sealed class PlatformResolver
    {
        private readonly ILogger<PlatformResolver> _logger;
        private readonly PackageDownloader _packageDownloader;

        public PlatformResolver(ILoggerFactory loggerFactory, PackageDownloader packageDownloader)
        {
	        _logger = loggerFactory.CreateLogger<PlatformResolver>();
			_packageDownloader = packageDownloader;
        }

        /// <summary>
        /// Gets all package dependencies of the given package for the given target framework. If the target is <c>null</c>, this returns all package dependencies.
        /// </summary>
        /// <param name="package">The package to examine.</param>
        /// <param name="target">The target platform. May be <c>null</c>.</param>
        public Dictionary<string, NugetPackageDependency> GetCompatiblePackageDependencies(NugetPackage package, PlatformTarget target)
        {
            _logger.DeterminingDependencies(package, target);

            var result = new Dictionary<string, NugetPackageDependency>(StringComparer.InvariantCultureIgnoreCase);
            var dependencies = target == null ?
                package.Metadata.NuspecReader.GetDependencyGroups().SelectMany(x => x.Packages) :
                NuGetFrameworkUtility.GetNearest(package.Metadata.NuspecReader.GetDependencyGroups(), NuGetFramework.ParseFrameworkName(target.FrameworkName.FullName, DefaultFrameworkNameProvider.Instance))?.Packages;
            if (dependencies == null)
            {
                _logger.NoDependenciesFound(package, target);
                return result;
            }
            var dependencyList = dependencies.ToList();
            _logger.AllDependencies(package, target, dependencyList);
            foreach (var dependency in dependencyList)
            {
                var versionRange = new NugetVersionRange(dependency.VersionRange);
                if (result.ContainsKey(dependency.Id))
                {
                    var merged = NugetVersionRange.TryMerge(result[dependency.Id].VersionRange, versionRange);
                    if (merged != null)
                        result[dependency.Id] = new NugetPackageDependency(dependency.Id, merged);
                    else
                        _logger.IncompatibleDependencyVersions(dependency.Id, result[dependency.Id].VersionRange, versionRange);
                }
                else
                {
                    result.Add(dependency.Id, new NugetPackageDependency(dependency.Id, versionRange));
                }
            }
            _logger.MergedDependencies(package, target, result.Values);
            return result;
        }

        /// <summary>
        /// Determines all platforms supported both by the specified package and by DotNetApis.
        /// This method may download package dependencies in order to determine the platform requirements of a package.
        /// </summary>
        /// <param name="package">The package to examine.</param>
        public async Task<PlatformTarget[]> AllSupportedPlatformsAsync(NugetPackage package)
        {
            _logger.DeterminingSupportedPlatforms(package);

            // If the package declares its platforms, just take those.
            var declaredPlatformSupport = DeclaredPlatforms(package);
            if (declaredPlatformSupport.Any())
                return declaredPlatformSupport;

            // Check all the dependencies for any supported platforms and merge them.
            _logger.NoImmediateSupportedPlatforms(package);
            var result = new List<PlatformTarget>();
            foreach (var dep in GetCompatiblePackageDependencies(package, null))
            {
                var depPackage = await _packageDownloader.TryGetPackageAsync(dep.Key, dep.Value.VersionRange).ConfigureAwait(false);
                if (depPackage == null)
                    continue;
                var depPlatforms = await AllSupportedPlatformsAsync(depPackage.Package).ConfigureAwait(false);
                foreach (var depPlatform in depPlatforms)
                {
                    var prefix = depPlatform.Prefix();
                    var existing = result.FindIndex(x => string.Equals(x.Prefix(), prefix, StringComparison.InvariantCultureIgnoreCase));
                    if (existing == -1)
                    {
                        result.Add(depPlatform);
                    }
                    else if (depPlatform.FrameworkName.Version > result[existing].FrameworkName.Version)
                    {
                        result[existing] = depPlatform;
                    }
                }
            }
            if (result.Count != 0)
                return result.OrderBy(x => x.NuGetFrameworkOrdering()).ThenByDescending(x => x.FrameworkName.Version).ToArray();

            // If the package has a dll in a plain "lib" directory, then just assume "net40".
            _logger.AttemptingFallbackPlatform(package);
            var guess = PlatformTarget.TryParse("net40");
            if (package.GetCompatibleAssemblyReferences(guess).Any())
                return new[] { guess };

            // Well, this looks like it might not be a .NET package at all...
            _logger.NoPlatformsFound(package);
            return new PlatformTarget[0];
        }

        private PlatformTarget[] DeclaredPlatforms(NugetPackage package)
        {
            var set = new HashSet<PlatformTarget>(EqualityComparerBuilder.For<PlatformTarget>().EquateBy(x => x.ToString()));
            foreach (var target in package.GetSupportedFrameworks())
            {
                if (target.FrameworkName.IsFrameworkPortable())
                {
                    foreach (var profileTargetString in target.ToString().Substring(9).Split('+'))
                    {
                        var profileTarget = PlatformTarget.TryParse(profileTargetString);
                        if (profileTarget == null)
                            _logger.FailedToParseProfileTarget(profileTargetString, target);
                        else
                            set.Add(profileTarget);
                    }
                }
                else
                {
                    set.Add(target);
                }
            }
            _logger.AllPlatforms(package, set);
            var result = set
                .Where(x => x.IsSupported())
                .OrderBy(x => x.NuGetFrameworkOrdering())
                .ThenByDescending(x => x.FrameworkName.Version)
                .ToArray();
            _logger.FilteredPlatforms(package, result);
            return result;
        }
    }

	internal static partial class Logging
	{
		public static void DeterminingDependencies(this ILogger<PlatformResolver> logger, NugetPackage package, PlatformTarget target) =>
			Logger.Log(logger, 1, LogLevel.Debug, "Determining package dependencies for {package} targeting {target}", package, target, null);

		public static void NoDependenciesFound(this ILogger<PlatformResolver> logger, NugetPackage package, PlatformTarget target) =>
			Logger.Log(logger, 2, LogLevel.Debug, "No dependencies found for {package} targeting {target}", package, target, null);

		public static void AllDependencies(this ILogger<PlatformResolver> logger, NugetPackage package, PlatformTarget target, IReadOnlyList<PackageDependency> dependencies) =>
			Logger.Log(logger, 3, LogLevel.Debug, "Full dependency list for {package} targeting {target} is {dependencies}", package, target, dependencies.Dumpable(), null);

		public static void IncompatibleDependencyVersions(this ILogger<PlatformResolver> logger, string dependencyId, NugetVersionRange versionRange1, NugetVersionRange versionRange2) =>
			Logger.Log(logger, 4, LogLevel.Warning, "NuGet client returned multiple incompatible version ranges for {dependencyId}: {versionRange1} and {versionRange2}", dependencyId, versionRange1, versionRange2, null);

		public static void MergedDependencies(this ILogger<PlatformResolver> logger, NugetPackage package, PlatformTarget target, IEnumerable<NugetPackageDependency> dependencies) =>
			Logger.Log(logger, 5, LogLevel.Debug, "Merged dependency list for {package} targeting {target} is {dependencies}", package, target, dependencies.Dumpable(), null);

		public static void DeterminingSupportedPlatforms(this ILogger<PlatformResolver> logger, NugetPackage package) =>
			Logger.Log(logger, 6, LogLevel.Debug, "Determining supported platforms for {package}", package, null);

		public static void NoImmediateSupportedPlatforms(this ILogger<PlatformResolver> logger, NugetPackage package) =>
			Logger.Log(logger, 7, LogLevel.Debug, "Found no declared platforms for {package}; checking dependencies for declared platforms", package, null);

		public static void AttemptingFallbackPlatform(this ILogger<PlatformResolver> logger, NugetPackage package) =>
			Logger.Log(logger, 8, LogLevel.Debug, "Package {package} does not have any dependencies with any supported platforms; trying desktop (net40) as a last resort", package, null);

		public static void NoPlatformsFound(this ILogger<PlatformResolver> logger, NugetPackage package) =>
			Logger.Log(logger, 9, LogLevel.Warning, "Failed to find any supported platforms for package {package}", package, null);

		public static void FailedToParseProfileTarget(this ILogger<PlatformResolver> logger, string profileTarget, PlatformTarget target) =>
			Logger.Log(logger, 10, LogLevel.Warning, "Ignoring {profileTarget} in {target} since it failed to parse", profileTarget, target, null);

		public static void AllPlatforms(this ILogger<PlatformResolver> logger, NugetPackage package, IEnumerable<PlatformTarget> targets) =>
			Logger.Log(logger, 11, LogLevel.Debug, "Package {package} declares support for platforms {targets}", package, targets.Dumpable(), null);

		public static void FilteredPlatforms(this ILogger<PlatformResolver> logger, NugetPackage package, IEnumerable<PlatformTarget> targets) =>
			Logger.Log(logger, 12, LogLevel.Debug, "After filtering, package {package} declares support for platforms {targets}", package, targets.Dumpable(), null);
	}
}
