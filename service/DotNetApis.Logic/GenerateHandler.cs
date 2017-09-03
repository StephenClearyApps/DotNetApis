using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetApis.Common;
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
        private readonly LogCombinedStorage _logStorage;
        private readonly PackageDownloader _packageDownloader;

        private NugetPackageIdVersion _idver;
        private PlatformTarget _target;
        private NugetPackage _currentPackage;

        public GenerateHandler(ILogger logger, LogCombinedStorage logStorage, PackageDownloader packageDownloader)
        {
            _logger = logger;
            _logStorage = logStorage;
            _packageDownloader = packageDownloader;
        }
        
        public async Task HandleAsync(GenerateRequestMessage message)
        {
            var idver = NugetPackageIdVersion.TryCreate(message.NormalizedPackageId, NugetVersion.TryParse(message.NormalizedPackageVersion));
            var target = PlatformTarget.TryParse(message.NormalizedFrameworkTarget);
            if (idver == null || target == null)
                throw new InvalidOperationException("Invalid generation request");
            try
            {
                _idver = idver;
                _target = target;
                await HandleAsync().ConfigureAwait(false);
                await _logStorage.WriteAsync(idver, target, message.Timestamp, Status.Succeeded, string.Join("\n", AmbientContext.InMemoryLogger?.Messages ?? new List<string>())).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(0, ex, "Error handling message {message}", JsonConvert.SerializeObject(message, Constants.JsonSerializerSettings));
                await _logStorage.WriteAsync(idver, target, message.Timestamp, Status.Failed, string.Join("\n", AmbientContext.InMemoryLogger?.Messages ?? new List<string>())).ConfigureAwait(false);
            }
        }

        private async Task HandleAsync()
        {
            // Load the package.
            var publishedPackage = await _packageDownloader.GetPackageAsync(_idver).ConfigureAwait(false);
            _currentPackage = publishedPackage.Package;

            // TODO: Determine all supported targets for the package.
        }
    }
}
