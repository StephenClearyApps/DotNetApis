using System;
using Microsoft.Azure;

namespace DotNetApis.Common
{
    public static class Config
    {
        /// <summary>
        /// Reads a config setting, returning <c>null</c> if the setting is not defined. This should only be called from within a lambda passed to <c>Singleton.Create</c>.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        public static string GetSetting(string name) => Environment.GetEnvironmentVariable(name) ?? CloudConfigurationManager.GetSetting(name, outputResultsToTrace: false);
    }
}
