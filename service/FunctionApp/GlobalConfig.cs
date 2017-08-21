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

        public static Container GetContainer()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            container.Options.DefaultLifestyle = Lifestyle.Scoped;
            container.Register<ILogger, Logger>();
            container.Register<INugetRepository, NugetRepository>();
            container.Register<DocRequestHandler>();
            container.Verify();
            return container;
        }
    }
}
