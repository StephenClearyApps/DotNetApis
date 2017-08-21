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

            Container = new Container();
            Container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            Container.Options.DefaultLifestyle = Lifestyle.Scoped;
            Container.Register<ILogger, AmbientCompositeLogger>();
            Container.Register<INugetRepository, NugetRepository>();
            Container.Register<DocRequestHandler>();
            Container.Verify();
        }

        public static void EnsureLoaded()
        {
        }

        public static Container Container { get; }
    }
}
