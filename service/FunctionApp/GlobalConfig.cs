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
        private static readonly Lazy<Container> _container;

        static GlobalConfig()
        {
            JsonConvert.DefaultSettings = () => Constants.JsonSerializerSettings;

            // Container setup is done behind a Lazy to prevent exceptions from taking down our process before error handling has been set up.
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
            Task.WhenAll(AzurePackageStorage.InitializeAsync(),
                AzurePackageTable.InitializeAsync()).GetAwaiter().GetResult();
        }

        public static void EnsureLoaded()
        {
        }

        public static Container Container => _container.Value;
    }
}
