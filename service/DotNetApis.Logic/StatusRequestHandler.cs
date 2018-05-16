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
        private readonly ILogger<StatusRequestHandler> _logger;
        private readonly IPackageJsonTable _packageJsonTable;
        private readonly Parser _parser;

        public StatusRequestHandler(ILoggerFactory loggerFactory, IPackageJsonTable packageJsonTable, Parser parser)
        {
            _logger = loggerFactory.CreateLogger<StatusRequestHandler>();
            _packageJsonTable = packageJsonTable;
            _parser = parser;
        }

        public (NugetPackageIdVersion idver, PlatformTarget target) NormalizeRequest(string packageId, string packageVersion, string targetFramework)
        {
            var idver = new NugetPackageIdVersion(packageId, _parser.ParseVersion(packageVersion));
            _logger.NormalizedPackage(packageId, packageVersion, idver);

            var target = _parser.ParsePlatformTarget(targetFramework);
            if (!target.IsSupported())
            {
                _logger.UnsupportedTargetFramework(targetFramework);
                throw new ExpectedException(HttpStatusCode.BadRequest, $"Target framework {targetFramework} is not supported");
            }
            _logger.NormalizedTargetFramework(targetFramework, target);

            return (idver, target);
        }

        public async Task<(Status Status, Uri LogUri, Uri JsonUri)?> TryGetStatusAsync(NugetPackageIdVersion idver, PlatformTarget target)
        {
            var result = await _packageJsonTable.TryGetRecordAsync(idver, target).ConfigureAwait(false);
            if (result == null)
            {
                _logger.StatusNotFound(idver, target);
                return null;
            }
            var (status, logUri, jsonUri) = result.Value;
            _logger.Status(idver, target, status, logUri, jsonUri);
            return result;
        }
    }

    internal static partial class Logging
    {
        public static void NormalizedPackage(this ILogger<StatusRequestHandler> logger, string packageId, string packageVersion, NugetPackageIdVersion idver) =>
            Logger.Log(logger, 1, LogLevel.Information, "Normalized package id {packageId} version {packageVersion} to {idver}", packageId, packageVersion, idver, null);

        public static void UnsupportedTargetFramework(this ILogger<StatusRequestHandler> logger, string targetFramework) =>
            Logger.Log(logger, 2, LogLevel.Error, "Target framework {targetFramework} is not supported", targetFramework, null);

        public static void NormalizedTargetFramework(this ILogger<StatusRequestHandler> logger, string targetFramework, PlatformTarget target) =>
            Logger.Log(logger, 3, LogLevel.Information, "Normalized target framework {targetFramework} to {target} ({targetFrameworkName})", targetFramework, target, target.FrameworkName, null);

        public static void StatusNotFound(this ILogger<StatusRequestHandler> logger, NugetPackageIdVersion idver, PlatformTarget target) =>
            Logger.Log(logger, 4, LogLevel.Debug, "Status for {idver} target {target} was not found", idver, target, null);

        public static void Status(this ILogger<StatusRequestHandler> logger, NugetPackageIdVersion idver, PlatformTarget target, Status status, Uri logUri, Uri jsonUri) =>
            Logger.Log(logger, 5, LogLevel.Debug, "Status for {idver} target {target} is {status}, {logUri}, {jsonUri}", idver, target, status, logUri, jsonUri, null);
    }
}
