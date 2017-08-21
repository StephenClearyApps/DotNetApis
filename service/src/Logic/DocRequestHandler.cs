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
        private readonly Logger _logger;
        private readonly NugetRepository _nugetRepository;

        public DocRequestHandler(Logger logger, NugetRepository nugetRepository)
        {
            _logger = logger;
            _nugetRepository = nugetRepository;
        }

        public string GetDoc(string packageId, string packageVersion, string targetFramework)
        {
            // Lookup the package version if unknown.
            var idver = packageVersion == null ? LookupLatestPackageVersion(packageId) : new NugetPackageIdVersion(packageId, ParseVersion(packageVersion));
            _logger.Trace($"Getting documentation for {idver}");

            // TODO: Determine the target framework if necessary.
            var target = ParsePlatformTarget(targetFramework);



            return idver + " " + target;
        }

        private NugetPackageIdVersion LookupLatestPackageVersion(string packageId)
        {
            var result = _nugetRepository.TryLookupLatestPackageVersion(packageId);
            if (result == null)
                throw new ExpectedException(HttpStatusCode.NotFound, $"Could not find package {packageId}");
            return result;
        }

        private static NugetVersion ParseVersion(string packageVersion)
        {
            var result = NugetVersion.TryParse(packageVersion);
            if (result == null)
                throw new ExpectedException(HttpStatusCode.BadRequest, $"Could not parse version {packageVersion}");
            return result;
        }

        private static PlatformTarget ParsePlatformTarget(string targetFramework)
        {
            var result = PlatformTarget.TryParse(targetFramework);
            if (result == null)
                throw new ExpectedException(HttpStatusCode.BadRequest, $"Could not parse target {targetFramework}");
            return result;
        }
    }
}
