using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Newtonsoft.Json;
using Nuget;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Storage;

namespace FunctionApp
{
    /// <summary>
    /// Apply app-wide configuration.
    /// </summary>
    public static class GlobalConfig
    {
        // All configuration is behind Lazy objects so we can control when exceptions get raised.
        private static readonly Lazy<object> _jsonSerializerSettings;
        private static readonly Lazy<Container> _container;
        private static readonly Lazy<Task> _azureInitialization;

        static GlobalConfig()
        {
            // Configure default JSON.NET serialization.
            _jsonSerializerSettings = new Lazy<object>(() =>
            {
                JsonConvert.DefaultSettings = () => Constants.JsonSerializerSettings;
                return null;
            });

            // Register DI implementations.
            _container = new Lazy<Container>(() =>
            {
                var container = new Container();
                container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
                container.Options.DefaultLifestyle = Lifestyle.Scoped;
                container.Register<ILogger, AmbientCompositeLogger>();
                container.Register<INugetRepository, NugetRepository>();
                container.Register<IPackageStorage, AzurePackageStorage>();
                container.Register<IPackageTable, AzurePackageTable>();
                container.Verify();
                return container;
            });

            // Initialize all components.
            _azureInitialization = new Lazy<Task>(() =>
                Task.WhenAll(AzurePackageStorage.InitializeAsync(),
                    AzurePackageTable.InitializeAsync())
            );
            var _ = _azureInitialization.Value;
        }

        public static void EnsureJsonSerializerSettings()
        {
            var _ = _jsonSerializerSettings.Value;
        }

        public static void EnsureInitilizationComplete()
        {
            var _ = _jsonSerializerSettings.Value;
            _azureInitialization.Value.GetAwaiter().GetResult();
            var __ = _container.Value;
        }

        public static Container Container => _container.Value;
    }
}
