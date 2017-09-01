using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;
using Nuget;
using Storage;
using System.Net.Http;

namespace Logic
{
    public sealed class StatusRequestHandler
    {
        private readonly ILogger _logger;
        private readonly IStatusTable _statusTable;

        public StatusRequestHandler(ILogger logger, IStatusTable statusTable)
        {
            _logger = logger;
            _statusTable = statusTable;
        }

        public (NugetPackageIdVersion idver, PlatformTarget target) NormalizeRequest(string packageId, string packageVersion, string targetFramework)
        {
            var idver = new NugetPackageIdVersion(packageId, ParseVersion(packageVersion));
            _logger.LogInformation("Normalized package id {packageId} version {packageVersion} to {idver}", packageId, packageVersion, idver);

            // Guess the target framework if unknown.
            var target = ParsePlatformTarget(targetFramework);
            if (!target.IsSupported())
            {
                _logger.LogError("Target framework {targetFramework} is not supported", targetFramework);
                throw new ExpectedException(HttpStatusCode.BadRequest, $"Target framework {targetFramework} is not supported");
            }
            _logger.LogInformation("Normalized target framework {targetFramework} to {target} ({targetFrameworkName})", targetFramework, target, target.FrameworkName);

            return (idver, target);
        }

        public async Task<(Status status, Uri logUri)?> TryGetStatusAsync(NugetPackageIdVersion idver, PlatformTarget target, DateTimeOffset timestamp)
        {
            var result = await _statusTable.TryGetStatusAsync(idver, target, timestamp).ConfigureAwait(false);
            if (result == null)
            {
                _logger.LogDebug("Status for {idver} target {target} on {timestamp} was not found", idver, target, timestamp);
                return null;
            }
            var (status, logUri) = result.Value;
            _logger.LogDebug("Status for {idver} target {target} on {timestamp} is {status}, {logUri}", idver, target, timestamp, status, logUri);
            return result;
        }

        private NugetVersion ParseVersion(string packageVersion) // TODO: Refactor to helper class
        {
            var result = NugetVersion.TryParse(packageVersion);
            if (result == null)
            {
                _logger.LogError("Could not parse version {packageVersion}", packageVersion);
                throw new ExpectedException(HttpStatusCode.BadRequest, $"Could not parse version `{packageVersion}`");
            }
            return result;
        }

        private PlatformTarget ParsePlatformTarget(string targetFramework)
        {
            var result = PlatformTarget.TryParse(targetFramework);
            if (result == null)
            {
                _logger.LogError("Could not parse target framework {targetFramework}", targetFramework);
                throw new ExpectedException(HttpStatusCode.BadRequest, $"Could not parse target framework `{targetFramework}`");
            }
            return result;
        }
    }
}
