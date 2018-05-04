using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Nuget;
using DotNetApis.Storage;
using Microsoft.Extensions.Logging;

namespace DotNetApis.Logic
{
    public sealed class DocRequestHandler
    {
        private readonly ILogger<DocRequestHandler> _logger;
        private readonly INugetRepository _nugetRepository;
        private readonly PackageDownloader _packageDownloader;
        private readonly PlatformResolver _platformResolver;
        private readonly PackageJsonCombinedStorage _packageJsonCombinedStorage;
        private readonly IPackageJsonTable _packageJsonTable;
        private readonly Parser _parser;

        public DocRequestHandler(ILoggerFactory loggerFactory, INugetRepository nugetRepository, PackageDownloader packageDownloader, PlatformResolver platformResolver,
            PackageJsonCombinedStorage packageJsonCombinedStorage, IPackageJsonTable packageJsonTable, Parser parser)
        {
            _logger = loggerFactory.CreateLogger<DocRequestHandler>();
            _nugetRepository = nugetRepository;
            _packageDownloader = packageDownloader;
            _platformResolver = platformResolver;
            _packageJsonCombinedStorage = packageJsonCombinedStorage;
            _packageJsonTable = packageJsonTable;
            _parser = parser;
        }

        public async Task<(NugetPackageIdVersion, PlatformTarget)> NormalizeRequestAsync(string packageId, string packageVersion, string targetFramework)
        {
            // Lookup the package version if unknown.
            var idver = packageVersion == null ? LookupLatestPackageVersion(packageId) : new NugetPackageIdVersion(packageId, _parser.ParseVersion(packageVersion));
            _logger.LogInformation("Normalized package id {packageId} version {packageVersion} to {idver}", packageId, packageVersion, idver);

            // Guess the target framework if unknown.
            var target = targetFramework == null ? await GuessPackageTargetAsync(idver).ConfigureAwait(false) : _parser.ParsePlatformTarget(targetFramework);
            if (!target.IsSupported())
            {
                _logger.LogError("Target framework {targetFramework} is not supported", targetFramework);
                throw new ExpectedException(HttpStatusCode.BadRequest, $"Target framework {targetFramework} is not supported");
            }
            _logger.LogInformation("Normalized target framework {targetFramework} to {target} ({targetFrameworkName})", targetFramework, target, target.FrameworkName);

            return (idver, target);
        }

        public async Task<(Uri JsonUri, Uri LogUri)> TryGetExistingJsonAndLogUriAsync(NugetPackageIdVersion idver, PlatformTarget target)
        {
            var result = await _packageJsonTable.TryGetRecordAsync(idver, target).ConfigureAwait(false);
            if (result == null)
                return (null, null);
            return (result.Value.JsonUri, result.Value.LogUri);
        }

        private NugetPackageIdVersion LookupLatestPackageVersion(string packageId)
        {
            var result = _nugetRepository.TryLookupLatestPackageVersion(packageId);
            if (result == null)
            {
                _logger.LogError("Could not find package {packageId}", packageId);
                throw new ExpectedException(HttpStatusCode.NotFound, $"Could not find package {packageId}");
            }
            return result;
        }

        private async Task<PlatformTarget> GuessPackageTargetAsync(NugetPackageIdVersion idver)
        {
            var package = await _packageDownloader.GetPackageAsync(idver).ConfigureAwait(false);

            var platforms = await _platformResolver.AllSupportedPlatformsAsync(package.Package).ConfigureAwait(false);
            var result = platforms.FirstOrDefault();
            if (result == null)
            {
                _logger.LogError("Package {package} has no supported frameworks", package);
                throw new ExpectedException(HttpStatusCode.BadRequest, $"Package `{package}` has no supported frameworks; the package possibly contains only source files, or is a JavaScript or other front-end package; DotNetApis only works with .NET packages");
            }

            return result;
        }
    }
}
