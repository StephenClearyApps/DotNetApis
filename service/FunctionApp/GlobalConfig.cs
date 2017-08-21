using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Logic;
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
        static GlobalConfig()
        {
            JsonConvert.DefaultSettings = () => Constants.JsonSerializerSettings;

            Container = new Container();
            Container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            Container.Options.DefaultLifestyle = Lifestyle.Scoped;
            Container.Register<ILogger, AmbientCompositeLogger>();
            Container.Register<INugetRepository, NugetRepository>();
            Container.Register<IPackageStorage, AzurePackageStorage>();
            Container.Register<IPackageTable, AzurePackageTable>();
            Container.Register<DocRequestHandler>();
            Container.Verify();

            // Initialize all components.
            Task.WhenAll(AzurePackageStorage.InitializeAsync(),
                AzurePackageTable.InitializeAsync()).GetAwaiter().GetResult();
        }

        public static void EnsureLoaded()
        {
        }

        public static Container Container { get; }
    }
}
