using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Logic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DotNetApis.Nuget;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using DotNetApis.Storage;

namespace FunctionApp
{
    /// <summary>
    /// Apply app-wide configuration.
    /// </summary>
    public static class GlobalConfig
    {
        // All configuration is behind Lazy objects so we can control when exceptions get raised.
        // These lazy instances use PublicationOnly to prevent caching of exceptions.
        private static readonly Lazy<object> _initialize;

        static GlobalConfig()
        {
            _initialize = new Lazy<object>(() =>
            {
                // Configure default JSON.NET serialization.
                JsonConvert.DefaultSettings = () => Constants.JsonSerializerSettings;

                // Use invariant culture by default.
                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

                return null;
            }, LazyThreadSafetyMode.PublicationOnly);
        }

        /// <summary>
        /// Ensures that all initialization has completed for this AppDomain. This method must be called from within a container scope.
        /// </summary>
        public static void Initialize()
        {
            // Ensure our current thread is using invariant culture.
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            var _ = _initialize.Value;
        }
    }
}
