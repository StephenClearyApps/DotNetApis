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
        private readonly ILogger<Parser> _logger;

        public Parser(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Parser>();
        }

        public NugetVersion ParseVersion(string packageVersion)
        {
            var result = NugetVersion.TryParse(packageVersion);
            if (result == null)
            {
                _logger.CannotParseVersion(packageVersion);
                throw new ExpectedException(HttpStatusCode.BadRequest, $"Could not parse version `{packageVersion}`");
            }
            return result;
        }

        public PlatformTarget ParsePlatformTarget(string targetFramework)
        {
            var result = PlatformTarget.TryParse(targetFramework);
            if (result == null)
            {
                _logger.CannotParseTarget(targetFramework);
                throw new ExpectedException(HttpStatusCode.BadRequest, $"Could not parse target framework `{targetFramework}`");
            }
            return result;
        }
    }

	internal static partial class Logging
	{
		public static void CannotParseVersion(this ILogger<Parser> logger, string packageVersion) =>
			Logger.Log(logger, 1, LogLevel.Error, "Could not parse version {packageVersion}", packageVersion, null);

		public static void CannotParseTarget(this ILogger<Parser> logger, string targetFramework) =>
			Logger.Log(logger, 1, LogLevel.Error, "Could not parse target framework {targetFramework}", targetFramework, null);
	}
}
