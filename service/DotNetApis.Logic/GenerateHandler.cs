using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Logic.Assemblies;
using DotNetApis.Logic.Formatting;
using DotNetApis.Logic.Messages;
using DotNetApis.Nuget;
using DotNetApis.Storage;
using DotNetApis.Structure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DotNetApis.Logic
{
    public sealed class GenerateHandler
    {
	    private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger<GenerateHandler> _logger;
        private readonly PackageDownloader _packageDownloader;
        private readonly PlatformResolver _platformResolver;
        private readonly NugetPackageDependencyResolver _dependencyResolver;
        private readonly Lazy<Task<ReferenceAssemblies>> _referenceAssemblies;
        private readonly IReferenceStorage _referenceStorage;
        private readonly AssemblyFormatter _assemblyFormatter;
        private readonly PackageJsonCombinedStorage _packageJsonCombinedStorage;
        private readonly INugetRepository _nugetRepository;

        public GenerateHandler(ILoggerFactory loggerFactory, PackageDownloader packageDownloader, PlatformResolver platformResolver,
            NugetPackageDependencyResolver dependencyResolver, Lazy<Task<ReferenceAssemblies>> referenceAssemblies, IReferenceStorage referenceStorage, AssemblyFormatter assemblyFormatter,
            PackageJsonCombinedStorage packageJsonCombinedStorage, INugetRepository nugetRepository)
        {
	        _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<GenerateHandler>();
            _packageDownloader = packageDownloader;
            _platformResolver = platformResolver;
            _dependencyResolver = dependencyResolver;
            _referenceAssemblies = referenceAssemblies;
            _referenceStorage = referenceStorage;
            _assemblyFormatter = assemblyFormatter;
            _packageJsonCombinedStorage = packageJsonCombinedStorage;
            _nugetRepository = nugetRepository;
        }
        
        public async Task HandleAsync(GenerateRequestMessage message)
        {
            var idver = NugetPackageIdVersion.TryCreate(message.NormalizedPackageId, NugetVersion.TryParse(message.NormalizedPackageVersion));
            var target = PlatformTarget.TryParse(message.NormalizedFrameworkTarget);
            if (idver == null || target == null)
                throw new InvalidOperationException("Invalid generation request");
            try
            {
                var doc = await HandleAsync(idver, target).ConfigureAwait(false);
                var docJson = JsonConvert.SerializeObject(doc, Constants.StorageJsonSerializerSettings);
                await _packageJsonCombinedStorage.WriteSuccessAsync(idver, target, docJson).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.HandlePackageException(message, ex);
                await _packageJsonCombinedStorage.WriteFailureAsync(idver, target).ConfigureAwait(false);
            }
        }

        private async Task<ReferenceAssemblies> LoadReferenceAssembliesAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            var result = await _referenceAssemblies.Value.ConfigureAwait(false);
            _logger.ReferenceAssembliesLoaded(stopwatch.Elapsed);
            return result;
        }

        private async Task<PackageJson> HandleAsync(NugetPackageIdVersion idver, PlatformTarget target)
        {
            var referenceAssembliesTask = LoadReferenceAssembliesAsync();

            // Load the package.
            var publishedPackage = await _packageDownloader.GetPackageAsync(idver).ConfigureAwait(false);
            var currentPackage = publishedPackage.Package;

            // Create the assembly collection for this request.
            var assemblies = new AssemblyCollection(_loggerFactory, currentPackage);

            // Determine all supported targets for the package.
            var allTargets = await _platformResolver.AllSupportedPlatformsAsync(currentPackage).ConfigureAwait(false);

            // Add assemblies for the current package to our context.
            foreach (var path in publishedPackage.Package.GetCompatibleAssemblyReferences(target))
                assemblies.AddCurrentPackageAssembly(path);
            _logger.DocumentingAssemblies(assemblies.CurrentPackageAssemblies);

            // Add assemblies for all dependency packages to our context.
            var dependencyPackages = await _dependencyResolver.ResolveAsync(publishedPackage.Package, target).ConfigureAwait(false);
            var dependencyAssemblyCount = 0;
            foreach (var dependentPackage in dependencyPackages)
            {
                foreach (var path in dependentPackage.GetCompatibleAssemblyReferences(target))
                {
                    ++dependencyAssemblyCount;
                    assemblies.AddDependencyPackageAssembly(dependentPackage, path);
                }
            }
            _logger.AddedDepencencyAssemblies(dependencyAssemblyCount, dependencyPackages.Count);

            // Sanity check: we'd better have something to generate documentation on.
            if (assemblies.CurrentPackageAssemblies.Count == 0 && assemblies.DependencyPackageAssemblies.Count == 0)
                throw new ExpectedException(HttpStatusCode.BadRequest, $"Neither package {idver} nor its dependencies have any assemblies for target {target}");

            // Add all target framework reference dlls to our context.
            var referenceAssemblies = await referenceAssembliesTask.ConfigureAwait(false);
            var referenceTarget = referenceAssemblies.ReferenceTargets.FirstOrDefault(x => NugetUtility.IsCompatible(x.Target.FrameworkName, target.FrameworkName));
            if (referenceTarget != null)
            {
                foreach (var path in referenceTarget.Paths.Where(x => x.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase)))
                    assemblies.AddReferenceAssembly(path, () => _referenceStorage.Download(path));
            }

            try
            {
                return GenerateAsync(target, assemblies, publishedPackage, dependencyPackages, allTargets);
            }
            catch (NeedsFSharpCoreException)
            {
                // https://fsharp.github.io/2015/04/18/fsharp-core-notes.html#a-c-project-referencing-an-f-dll-or-nuget-package-may-need-to-also-have-a-reference-to-fsharpcoredll
                _logger.DetectedFSharp();

                // Attempt to load all versions of FSharp.Core from highest to lowest until we find one compatible with this target.
                var found = false;
                var versions = _nugetRepository.GetPackageVersions("FSharp.Core");
                foreach (var version in versions.Where(x => !x.IsPrerelease).Concat(versions.Where(x => x.IsPrerelease)))
                {
                    var fsharpIdver = new NugetPackageIdVersion("FSharp.Core", version);
                    var fsharp = await _packageDownloader.GetPackageAsync(fsharpIdver).ConfigureAwait(false);
                    var compatibleReferences = fsharp.Package.GetCompatibleAssemblyReferences(target).ToList();
                    if (compatibleReferences.Count == 0)
                    {
                        _logger.FSharpCoreNotCompatibleWithTarget(fsharpIdver, target);
                    }
                    else
                    {
                        found = true;
                        foreach (var path in compatibleReferences)
                            assemblies.AddDependencyPackageAssembly(fsharp.Package, path);
                        break;
                    }
                }
                if (!found)
                {
                    _logger.FSharpCoreNotFound();
                    throw new ExpectedException(HttpStatusCode.BadRequest, "Could not find compatible version of FSharp.Core");
                }
                return GenerateAsync(target, assemblies, publishedPackage, dependencyPackages, allTargets);
            }
        }

        private PackageJson GenerateAsync(PlatformTarget target, AssemblyCollection assemblies, NugetFullPackage publishedPackage, IEnumerable<NugetPackage> dependencyPackages, IEnumerable<PlatformTarget> allTargets)
        {
            using (GenerationScope.Create(target, assemblies))
            {
                // Produce JSON structure for all package dependencies.
                var immediateDependencies = _platformResolver.GetCompatiblePackageDependencies(publishedPackage.Package, target);
                var dependencies = dependencyPackages.Select(x =>
                {
                    immediateDependencies.TryGetValue(x.Metadata.PackageId, out var immediateDependency);
                    return new PackageDependencyJson
                    {
                        Title = x.Metadata.Title,
                        PackageId = x.Metadata.PackageId,
                        VersionRange = immediateDependency?.VersionRange?.ToString(),
                        Version = x.Metadata.Version.ToString(),
                        Summary = x.Metadata.Summary,
                        Authors = x.Metadata.Authors,
                        IconUrl = x.Metadata.IconUrl,
                        ProjectUrl = x.Metadata.ProjectUrl,
                    };
                }).OrderBy(x => x.VersionRange == null).ThenBy(x => x.PackageId, StringComparer.InvariantCultureIgnoreCase).ToList();

                // Produce JSON structure for the primary package.
                return new PackageJson
                {
                    PackageId = publishedPackage.Package.Metadata.PackageId,
                    Version = publishedPackage.Package.Metadata.Version.ToString(),
                    Target = target.ToString(),
                    Description = publishedPackage.Package.Metadata.Description,
                    Authors = publishedPackage.Package.Metadata.Authors,
                    IconUrl = publishedPackage.Package.Metadata.IconUrl,
                    ProjectUrl = publishedPackage.Package.Metadata.ProjectUrl,
                    SupportedTargets = allTargets.Select(x => x.ToString()).ToList(),
                    Dependencies = dependencies,
                    Published = publishedPackage.ExternalMetadata.Published,
                    IsReleaseVersion = publishedPackage.Package.Metadata.Version.IsReleaseVersion,
                    Assemblies = assemblies.CurrentPackageAssemblies.Where(x => x.AssemblyDefinition != null).Select(_assemblyFormatter.Assembly).ToList(),
                };
            }
        }
    }

	internal static partial class Logging
	{
		public static void HandlePackageException(this ILogger<GenerateHandler> logger, GenerateRequestMessage message, Exception exception) =>
			Logger.Log(logger, 1, LogLevel.Critical, "Error handling message {message}", message, exception);

		public static void ReferenceAssembliesLoaded(this ILogger<GenerateHandler> logger, TimeSpan elapsed) =>
			Logger.Log(logger, 2, LogLevel.Debug, "Reference assemblies loaded in {elapsed}", elapsed, null);

		public static void DocumentingAssemblies(this ILogger<GenerateHandler> logger, IReadOnlyList<CurrentPackageAssembly> assemblies) =>
			Logger.Log(logger, 3, LogLevel.Debug, "Documentation will be generated for {assemblies}", assemblies.Dumpable(), null);

		public static void AddedDepencencyAssemblies(this ILogger<GenerateHandler> logger, int assemblyCount, int packageCount) =>
			Logger.Log(logger, 4, LogLevel.Debug, "Added {assemblyCount} assemblies from {packageCount} dependency packages", assemblyCount, packageCount, null);

		public static void DetectedFSharp(this ILogger<GenerateHandler> logger) =>
			Logger.Log(logger, 5, LogLevel.Information, "Detected implicit dependency on FSharp.Core. Retrying...", null);

		public static void FSharpCoreNotCompatibleWithTarget(this ILogger<GenerateHandler> logger, NugetPackageIdVersion idver, PlatformTarget target) =>
			Logger.Log(logger, 6, LogLevel.Debug, "Rejecting {idver} because it does not support target {target}", idver, target, null);

		public static void FSharpCoreNotFound(this ILogger<GenerateHandler> logger) =>
			Logger.Log(logger, 7, LogLevel.Error, "Could not find compatible version of FSharp.Core", null);
	}
}
