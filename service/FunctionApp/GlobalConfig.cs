using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;
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
                container.Register<AzureConnections>(Lifestyle.Singleton);
                container.Register<ILogger, AmbientCompositeLogger>();
                container.Register<INugetRepository, NugetRepository>();
                container.Register<IPackageStorage, AzurePackageStorage>();
                container.Register<IPackageTable, AzurePackageTable>();
                container.Register<IPackageJsonTable, AzurePackageJsonTable>();
                container.Register<IPackageJsonStorage, AzurePackageJsonStorage>();
                container.Verify();
                return container;
            });

            // Initialize all components.
            _azureInitialization = new Lazy<Task>(async () =>
            {
                var connections = Container.GetInstance<AzureConnections>();
                await connections.InitializeAsync().ConfigureAwait(false);
                await Task.WhenAll(AzurePackageStorage.InitializeAsync(),
                        AzurePackageTable.InitializeAsync(),
                        AzurePackageJsonTable.InitializeAsync(),
                        AzurePackageJsonStorage.InitializeAsync(connections))
                    .ConfigureAwait(false);
            });
        }

        /// <summary>
        /// Ensures that the JSON.NET serializer settings have been set for this AppDomain.
        /// </summary>
        public static void EnsureJsonSerializerSettings()
        {
            var _ = _jsonSerializerSettings.Value;
        }

        /// <summary>
        /// Ensures that all initialization has completed for this AppDomain. This method must be called from within a container scope.
        /// </summary>
        public static async Task EnsureInitilizationCompleteAsync()
        {
            var _ = _jsonSerializerSettings.Value;
            var __ = _container.Value;
            await _azureInitialization.Value.ConfigureAwait(false);
        }

        public static Container Container => _container.Value;
    }
}
