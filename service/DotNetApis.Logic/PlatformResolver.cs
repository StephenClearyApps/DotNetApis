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

namespace DotNetApis.Logic
{
    public sealed class PlatformResolver
    {
        private readonly ILogger _logger;
        private readonly PackageDownloader _packageDownloader;

        public PlatformResolver(ILogger logger, PackageDownloader packageDownloader)
        {
            _logger = logger;
            _packageDownloader = packageDownloader;
        }

        /// <summary>
        /// Gets all package dependencies of the given package for the given target framework. If the target is <c>null</c>, this returns all package dependencies.
        /// </summary>
        /// <param name="package">The package to examine.</param>
        /// <param name="target">The target platform. May be <c>null</c>.</param>
        public Dictionary<string, NugetPackageDependency> GetCompatiblePackageDependencies(NugetPackage package, PlatformTarget target)
        {
            _logger.LogDebug("Determining package dependencies for {package} targeting {target}", package, target);

            var result = new Dictionary<string, NugetPackageDependency>(StringComparer.InvariantCultureIgnoreCase);
            var dependencies = target == null ?
                package.Metadata.NuspecReader.GetDependencyGroups().SelectMany(x => x.Packages) :
                NuGetFrameworkUtility.GetNearest(package.Metadata.NuspecReader.GetDependencyGroups(), NuGetFramework.ParseFrameworkName(target.FrameworkName.FullName, DefaultFrameworkNameProvider.Instance))?.Packages;
            if (dependencies == null)
            {
                _logger.LogDebug("No dependencies found for {package} targeting {target}", package, target);
                return result;
            }
            var dependencyList = dependencies.ToList();
            _logger.LogDebug("Full dependency list for {package} targeting {target} is {dependencyList}", package, target, dependencyList.Dump());
            foreach (var dependency in dependencyList)
            {
                var versionRange = new NugetVersionRange(dependency.VersionRange);
                if (result.ContainsKey(dependency.Id))
                {
                    var merged = NugetVersionRange.TryMerge(result[dependency.Id].VersionRange, versionRange);
                    if (merged != null)
                        result[dependency.Id] = new NugetPackageDependency(dependency.Id, merged);
                    else
                        _logger.LogWarning("NuGet client returned multiple incompatible version ranges for {dependencyId}: {versionRange1} and {versionRange2}",
                            dependency.Id, result[dependency.Id].VersionRange, versionRange);
                }
                else
                {
                    result.Add(dependency.Id, new NugetPackageDependency(dependency.Id, versionRange));
                }
            }
            _logger.LogDebug("Merged dependency list for {package} targeting {target} is {dependencyList}", package, target, result.Values.Dump());
            return result;
        }

        /// <summary>
        /// Determines all platforms supported both by the specified package and by DotNetApis.
        /// This method may download package dependencies in order to determine the platform requirements of a package.
        /// </summary>
        /// <param name="package">The package to examine.</param>
        public async Task<PlatformTarget[]> AllSupportedPlatformsAsync(NugetPackage package)
        {
            _logger.LogDebug("Determining supported platforms for {package}", package);

            // If the package declares its platforms, just take those.
            var declaredPlatformSupport = DeclaredPlatforms(package);
            if (declaredPlatformSupport.Any())
                return declaredPlatformSupport;

            // Check all the dependencies for any supported platforms and merge them.
            _logger.LogDebug("Found no declared platforms for {package}; checking dependencies for declared platforms", package);
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
            _logger.LogDebug("Package {package} does not have any dependencies with any supported platforms; trying desktop (net40) as a last resort", package);
            var guess = PlatformTarget.TryParse("net40");
            if (package.GetCompatibleAssemblyReferences(guess).Any())
                return new[] { guess };

            // Well, this looks like it might not be a .NET package at all...
            _logger.LogWarning("Failed to find any supported platforms for package {package}", package);
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
                            _logger.LogWarning("Ignoring {profileTargetString} in {platformTarget} since it failed to parse", profileTargetString, target);
                        else
                            set.Add(profileTarget);
                    }
                }
                else
                {
                    set.Add(target);
                }
            }
            _logger.LogDebug("Package {package} declares support for platforms {platforms}", package, set.Dump());
            var result = set
                .Where(x => x.IsSupported())
                .OrderBy(x => x.NuGetFrameworkOrdering())
                .ThenByDescending(x => x.FrameworkName.Version)
                .ToArray();
            _logger.LogDebug("After filtering, package {package} declares support for platforms {platforms}", package, result.Dump());
            return result;
        }
    }
}
