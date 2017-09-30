using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetApis.Common;
using Newtonsoft.Json;

namespace FunctionApp
{
    /// <summary>
    /// Apply app-wide configuration.
    /// </summary>
    public static class GlobalConfig
    {
        /// <summary>
        /// Ensures that all initialization has completed for this AppDomain.
        /// </summary>
        public static void Initialize()
        {
            SetDefaultJsonNetSerialization.EnsureInitialized();
            SetDefaultThreadCurrentCulture.EnsureInitialized();

            // Ensure our current thread is using invariant culture.
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        }

        private static readonly ISingleton<object> SetDefaultJsonNetSerialization = Singleton.Create(() =>
        {
            // Configure default JSON.NET serialization.
            JsonConvert.DefaultSettings = () => Constants.JsonSerializerSettings;
        });

        private static readonly ISingleton<object> SetDefaultThreadCurrentCulture = Singleton.Create(() =>
        {
            // Use invariant culture by default.
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        });
    }
}
