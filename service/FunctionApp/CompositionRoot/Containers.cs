using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Logic;
using DotNetApis.Nuget;
using DotNetApis.SimpleInjector;
using DotNetApis.Storage;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using static FunctionApp.CompositionRoot.Singletons;

namespace FunctionApp.CompositionRoot
{
    public static class Containers
    {
        public static async Task<Container> GetContainerAsync()
        {
            try
            {
                return await Container;
            }
            catch (Exception ex)
            {
                AsyncLocalLogger.Logger.LogCritical(0, ex, "Failed to create container composition root");
                throw;
            }
        }

        public static Container GetContainerForNugetSearch()
        {
            try
            {
                return NugetSearchContainer.Value;
            }
            catch (Exception ex)
            {
                AsyncLocalLogger.Logger.LogCritical(0, ex, "Failed to create container composition root");
                throw;
            }
        }

        private static readonly ISingleton<Container> NugetSearchContainer = Singleton.Create(() =>
        {
            var container = CreateContainer();
            container.Register<ILogger, AsyncLocalLogger>();
            container.Register<NugetSearchFunction>();
            container.Verify();
            return container;
        });

        private static readonly IAsyncSingleton<Container> Container = Singleton.Create(async () =>
        {
            var singletons = await AsyncTupleHelpers.WhenAll(ReferenceStorageInstance.Value, CloudBlobClientInstance.Value, PackageStorageCloudBlobContainer.Value,
                PackageTableCloudTable.Value, PackageJsonTableCloudTable.Value, PackageJsonStorageCloudBlobContainer.Value, ReferenceXmldocTableCloudTable.Value);

            var container = CreateContainer();
            container.RegisterSingletons((CloudStorageAccountInstance.Value, CloudTableClientInstance.Value));
            container.RegisterSingletons(singletons);
            container.Register(() => new Lazy<Task<ReferenceAssemblies>>(async () => await ReferenceAssembliesInstance));
            container.Register<ILogger, AsyncLocalLogger>();
            container.Register<INugetRepository, NugetRepository>();
            container.Register<IStorageBackend, AzureStorageBackend>();
            container.Register<ILogStorage, AzureLogStorage>();
            container.Register<IStatusTable, AzureStatusTable>();
            container.Register<IPackageStorage, AzurePackageStorage>();
            container.Register<IPackageTable, AzurePackageTable>();
            container.Register<IPackageJsonTable, AzurePackageJsonTable>();
            container.Register<IPackageJsonStorage, AzurePackageJsonStorage>();
            container.Register<IReferenceXmldocTable, AzureReferenceXmldocTable>();

            // Best practice: register all root elements explicitly, so missing registrations are found immediately.
            container.Register<DocumentationFunction>();
            container.Register<GenerateFunction>();
            container.Register<StatusFunction>();
            container.Register<OpsFunction>();

            container.Verify();
            return container;
        });

        private static Container CreateContainer()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            container.Options.DefaultLifestyle = Lifestyle.Scoped;
            container.UseAutomaticInstanceOf();
            return container;
        }
    }
}
