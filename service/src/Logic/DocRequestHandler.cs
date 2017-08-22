using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;
using Nuget;

namespace Logic
{
    public sealed class DocRequestHandler
    {
        private readonly ILogger _logger;
        private readonly INugetRepository _nugetRepository;
        private readonly PackageDownloader _packageDownloader;
        private readonly PlatformResolver _platformResolver;

        public DocRequestHandler(ILogger logger, INugetRepository nugetRepository, PackageDownloader packageDownloader, PlatformResolver platformResolver)
        {
            _logger = logger;
            _nugetRepository = nugetRepository;
            _packageDownloader = packageDownloader;
            _platformResolver = platformResolver;
        }

        public async Task<string> GetDocAsync(string packageId, string packageVersion, string targetFramework)
        {
            // Lookup the package version if unknown.
            var idver = packageVersion == null ? LookupLatestPackageVersion(packageId) : new NugetPackageIdVersion(packageId, ParseVersion(packageVersion));
            _logger.Trace($"Getting documentation for `{idver}`");

            var target = targetFramework == null ? await GuessPackageTargetAsync(idver) : ParsePlatformTarget(targetFramework);



            return idver + " " + target;
        }

        private NugetPackageIdVersion LookupLatestPackageVersion(string packageId)
        {
            var result = _nugetRepository.TryLookupLatestPackageVersion(packageId);
            if (result == null)
                throw new ExpectedException(HttpStatusCode.NotFound, $"Could not find package `{packageId}`");
            return result;
        }

        private async Task<PlatformTarget> GuessPackageTargetAsync(NugetPackageIdVersion idver)
        {
            var package = await _packageDownloader.GetPackageAsync(idver).ConfigureAwait(false);

            var platforms = await _platformResolver.AllSupportedPlatformsAsync(package.Package).ConfigureAwait(false);
            var result = platforms.FirstOrDefault();
            if (result == null)
                throw new ExpectedException(HttpStatusCode.BadRequest, $"Package `{package}` has no supported frameworks; the package possibly contains only source files, or is a JavaScript or other front-end package; DotNetApis only works with .NET packages");

            return result;
        }

        private static NugetVersion ParseVersion(string packageVersion)
        {
            var result = NugetVersion.TryParse(packageVersion);
            if (result == null)
                throw new ExpectedException(HttpStatusCode.BadRequest, $"Could not parse version `{packageVersion}`");
            return result;
        }

        private PlatformTarget ParsePlatformTarget(string targetFramework)
        {
            var result = PlatformTarget.TryParse(targetFramework);
            if (result == null)
                throw new ExpectedException(HttpStatusCode.BadRequest, $"Could not parse target `{targetFramework}`");
            _logger.Trace($"Normalized target framework `{targetFramework}` to {result} ({result.FrameworkName})");
            return result;
        }
    }
}
