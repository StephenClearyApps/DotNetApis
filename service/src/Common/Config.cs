using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;

namespace Common
{
    public static class Config
    {
        public static string GetSetting(string name) => Environment.GetEnvironmentVariable(name) ?? CloudConfigurationManager.GetSetting(name, outputResultsToTrace: false);
    }
}
