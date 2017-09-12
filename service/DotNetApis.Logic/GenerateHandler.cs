using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Logic.Assemblies;
using DotNetApis.Logic.Messages;
using DotNetApis.Nuget;
using DotNetApis.Storage;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DotNetApis.Logic
{
    public sealed class GenerateHandler
    {
        private readonly ILogger _logger;
        private readonly InMemoryLogger _inMemoryLogger;
        private readonly LogCombinedStorage _logStorage;
        private readonly PackageDownloader _packageDownloader;
        private readonly PlatformResolver _platformResolver;
        private readonly NugetPackageDependencyResolver _dependencyResolver;

        public GenerateHandler(ILogger logger, InMemoryLogger inMemoryLogger, LogCombinedStorage logStorage, PackageDownloader packageDownloader, PlatformResolver platformResolver, NugetPackageDependencyResolver dependencyResolver)
        {
            _logger = logger;
            _logStorage = logStorage;
            _packageDownloader = packageDownloader;
            _platformResolver = platformResolver;
            _dependencyResolver = dependencyResolver;
            _inMemoryLogger = inMemoryLogger;
        }
        
        public async Task HandleAsync(GenerateRequestMessage message)
        {
            var idver = NugetPackageIdVersion.TryCreate(message.NormalizedPackageId, NugetVersion.TryParse(message.NormalizedPackageVersion));
            var target = PlatformTarget.TryParse(message.NormalizedFrameworkTarget);
            if (idver == null || target == null)
                throw new InvalidOperationException("Invalid generation request");
            try
            {
                await HandleAsync(idver, target).ConfigureAwait(false);
                await _logStorage.WriteAsync(idver, target, message.Timestamp, Status.Succeeded, string.Join("\n", _inMemoryLogger?.Messages ?? new List<string>())).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(0, ex, "Error handling message {message}", JsonConvert.SerializeObject(message, Constants.JsonSerializerSettings));
                await _logStorage.WriteAsync(idver, target, message.Timestamp, Status.Failed, string.Join("\n", _inMemoryLogger?.Messages ?? new List<string>())).ConfigureAwait(false);
            }
        }

        private async Task HandleAsync(NugetPackageIdVersion idver, PlatformTarget target)
        {
            // Load the package.
            var publishedPackage = await _packageDownloader.GetPackageAsync(idver).ConfigureAwait(false);
            var currentPackage = publishedPackage.Package;

            // Create the assembly collection for this request.
            var assemblies = new AssemblyCollection(_logger, currentPackage);

            // Determine all supported targets for the package.
            var allTargets = await _platformResolver.AllSupportedPlatformsAsync(currentPackage).ConfigureAwait(false);

            // Add assemblies for the current package to our context.
            foreach (var path in publishedPackage.Package.GetCompatibleAssemblyReferences(target))
                assemblies.AddCurrentPackageAssembly(path);
            _logger.LogDebug("Documentation will be generated for {assemblies}", assemblies.CurrentPackageAssemblies.Dump());

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
            _logger.LogDebug("Added {assemblyCount} assemblies from {packageCount} dependency packages", dependencyAssemblyCount, dependencyPackages.Count);

            // Sanity check: we'd better have something to generate documentation on.
            if (assemblies.CurrentPackageAssemblies.Count == 0 && assemblies.DependencyPackageAssemblies.Count == 0)
                throw new ExpectedException(HttpStatusCode.BadRequest, $"Neither package {idver} nor its dependencies have any assemblies for target {target}");

            throw new NotImplementedException();
        }
    }
}
