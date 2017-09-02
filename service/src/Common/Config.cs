using System;
using Microsoft.Azure;

namespace DotNetApis.Common
{
    public static class Config
    {
        public static string GetSetting(string name) => Environment.GetEnvironmentVariable(name) ?? CloudConfigurationManager.GetSetting(name, outputResultsToTrace: false);
    }
}
