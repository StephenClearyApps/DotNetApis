using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Newtonsoft.Json;

namespace FunctionApp
{
    /// <summary>
    /// Apply app-wide configuration.
    /// </summary>
    public static class GlobalConfig
    {
        static GlobalConfig()
        {
            JsonConvert.DefaultSettings = () => Constants.JsonSerializerSettings;
        }

        public static void EnsureLoaded()
        {
        }
    }
}
