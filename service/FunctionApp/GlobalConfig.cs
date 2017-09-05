using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetApis.Common;
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
        private static readonly Lazy<Container> _container;
        private static readonly Lazy<object> _initialize;

        static GlobalConfig()
        {
            _container = new Lazy<Container>(() =>
            {
                var container = new Container();
                container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
                container.Options.DefaultLifestyle = Lifestyle.Scoped; 
                container.Register<AzureConnections>();
                container.Register<IReferenceStorage, AzureReferenceStorage>();
                container.Register<ILogger, AmbientCompositeLogger>();
                container.Register<INugetRepository, NugetRepository>();
                container.Register<IPackageStorage, AzurePackageStorage>();
                container.Register<IPackageTable, AzurePackageTable>();
                container.Register<IPackageJsonTable, AzurePackageJsonTable>();
                container.Register<IPackageJsonStorage, AzurePackageJsonStorage>();
                container.Register<ILogStorage, AzureLogStorage>();
                container.Register<IStatusTable, AzureStatusTable>();
                container.Verify();
                return container;
            }, LazyThreadSafetyMode.PublicationOnly);

            _initialize = new Lazy<object>(() =>
            {
                // Configure default JSON.NET serialization.
                JsonConvert.DefaultSettings = () => Constants.JsonSerializerSettings;

                // Use invariant culture by default.
                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

                // Initialize all Azure resources.
                InitializeAzureResourcesAsync().GetAwaiter().GetResult();

                return null;
            }, LazyThreadSafetyMode.PublicationOnly);
        }

        public static Container Container => _container.Value;

        /// <summary>
        /// Ensures that all initialization has completed for this AppDomain. This method must be called from within a container scope.
        /// </summary>
        public static void Initialize()
        {
            // Ensure our current thread is using invariant culture.
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            var _ = _initialize.Value;
        }

        private static async Task InitializeAzureResourcesAsync()
        {
            var connections = Container.GetInstance<AzureConnections>();
            await connections.InitializeAsync().ConfigureAwait(false);
            await Task.WhenAll(AzurePackageStorage.InitializeAsync(connections),
                    AzurePackageTable.InitializeAsync(connections),
                    AzurePackageJsonTable.InitializeAsync(connections),
                    AzurePackageJsonStorage.InitializeAsync(connections),
                    AzureReferenceStorage.InitializeAsync(connections))
                .ConfigureAwait(false);
        }
    }
}
