using System;
using System.Net;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Nuget;
using DotNetApis.Storage;
using Microsoft.Extensions.Logging;

namespace DotNetApis.Logic
{
    public sealed class StatusRequestHandler
    {
        private readonly ILogger _logger;
        private readonly IStatusTable _statusTable;
        private readonly Parser _parser;

        public StatusRequestHandler(ILogger logger, IStatusTable statusTable, Parser parser)
        {
            _logger = logger;
            _statusTable = statusTable;
            _parser = parser;
        }

        public (NugetPackageIdVersion idver, PlatformTarget target) NormalizeRequest(string packageId, string packageVersion, string targetFramework)
        {
            var idver = new NugetPackageIdVersion(packageId, _parser.ParseVersion(packageVersion));
            _logger.LogInformation("Normalized package id {packageId} version {packageVersion} to {idver}", packageId, packageVersion, idver);

            var target = _parser.ParsePlatformTarget(targetFramework);
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
    }
}
