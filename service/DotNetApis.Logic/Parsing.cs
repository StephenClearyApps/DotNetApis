using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Nuget;
using Microsoft.Extensions.Logging;

namespace DotNetApis.Logic
{
    public sealed class Parser
    {
        private readonly ILogger _logger;

        public Parser(ILogger logger)
        {
            _logger = logger;
        }

        public NugetVersion ParseVersion(string packageVersion)
        {
            var result = NugetVersion.TryParse(packageVersion);
            if (result == null)
            {
                _logger.LogError("Could not parse version {packageVersion}", packageVersion);
                throw new ExpectedException(HttpStatusCode.BadRequest, $"Could not parse version `{packageVersion}`");
            }
            return result;
        }

        public PlatformTarget ParsePlatformTarget(string targetFramework)
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
