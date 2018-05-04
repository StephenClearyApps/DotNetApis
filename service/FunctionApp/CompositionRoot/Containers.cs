using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Nuget;
using DotNetApis.SimpleInjector;
using DotNetApis.Storage;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using static FunctionApp.CompositionRoot.Singletons;
using static DotNetApis.Common.AsyncTupleHelpers;

namespace FunctionApp.CompositionRoot
{
    public static class Containers
    {
        static Containers()
        {
            ContainerFor<NugetSearchFunction>.Create();
            ContainerFor<OpsFunction>.Create(async container =>
            {
                var singletons = await WhenAll(ReferenceStorageInstance.Value, ReferenceXmldocTableCloudTable.Value);
                container.RegisterInstances(singletons);
                container.RegisterInstance(CloudStorageAccountInstance.Value);
                container.RegisterLazyTask(() => ReferenceAssembliesInstance.Value);
                container.Register<IReferenceXmldocTable, AzureReferenceXmldocTable>();
                container.Register<IStorageBackend, AzureStorageBackend>();
            });
            ContainerFor<StatusFunction>.Create(async container =>
            {
                container.RegisterInstance(await PackageJsonTableCloudTable);
                container.RegisterInstance(CloudTableClientInstance.Value);
                container.Register<IPackageJsonTable, AzurePackageJsonTable>();
            });
            ContainerFor<DocumentationFunction>.Create(async container =>
            {
                var singletons = await WhenAll(PackageTableCloudTable.Value, PackageStorageCloudBlobContainer.Value, PackageJsonTableCloudTable.Value, PackageJsonStorageCloudBlobContainer.Value);
                container.RegisterInstances(singletons);
                container.RegisterInstance(CloudTableClientInstance.Value);
                container.Register<INugetRepository, NugetRepository>();
                container.Register<IPackageTable, AzurePackageTable>();
                container.Register<IPackageStorage, AzurePackageStorage>();
                container.Register<IPackageJsonTable, AzurePackageJsonTable>();
                container.Register<IPackageJsonStorage, AzurePackageJsonStorage>();
            });
            ContainerFor<GenerateFunction>.Create(async container =>
            {
                var singletons = await WhenAll(CloudBlobClientInstance.Value, PackageTableCloudTable.Value, PackageStorageCloudBlobContainer.Value, ReferenceStorageInstance.Value,
                    ReferenceXmldocTableCloudTable.Value, PackageJsonTableCloudTable.Value, PackageJsonStorageCloudBlobContainer.Value);
                container.RegisterInstances(singletons);
                container.RegisterInstance(CloudTableClientInstance.Value);
                container.RegisterLazyTask(() => ReferenceAssembliesInstance.Value);
                container.Register<IPackageTable, AzurePackageTable>();
                container.Register<IPackageStorage, AzurePackageStorage>();
                container.Register<INugetRepository, NugetRepository>();
                container.Register<IReferenceXmldocTable, AzureReferenceXmldocTable>();
                container.Register<IPackageJsonTable, AzurePackageJsonTable>();
                container.Register<IPackageJsonStorage, AzurePackageJsonStorage>();
            });

#if DEBUG
            // When in debug mode, force all containers to validate as soon as any container is requested.
            ContainerFor<NugetSearchFunction>.GetAsync().Wait();
            ContainerFor<OpsFunction>.GetAsync().Wait();
            ContainerFor<StatusFunction>.GetAsync().Wait();
            ContainerFor<DocumentationFunction>.GetAsync().Wait();
            ContainerFor<GenerateFunction>.GetAsync().Wait();
#endif
        }

        public static async Task<Container> GetContainerForAsync<T>() where T : class
        {
            try
            {
                return await ContainerFor<T>.GetAsync();
            }
            catch (Exception ex)
            {
                AsyncLocalLoggerFactory.LoggerFactory.CreateLogger("Containers").LogCritical(0, ex, "Failed to create container composition root");
                throw;
            }
        }

        private static class ContainerFor<T>
            where T : class
        {
            // ReSharper disable once StaticMemberInGenericType
            private static IAsyncSingleton<Container> _instance;

            public static void Create(Func<Container, Task> registrations)
            {
                _instance = Singleton.Create(async () =>
                {
                    var container = new Container();
                    container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
                    container.Options.DefaultLifestyle = Lifestyle.Scoped;

                    // Registrations common for all containers
                    container.UseAutomaticInstanceOf();
                    container.Register<ILoggerFactory, AsyncLocalLoggerFactory>();

                    // Container-specific registrations
                    await registrations(container);

                    // Best practice: register the root element explicitly, so missing registrations are found immediately.
                    container.Register<T>();

                    container.Verify();
                    return container;
                });
            }

#pragma warning disable 1998
            public static void Create(Action<Container> registrations = null) => Create(async container => registrations?.Invoke(container));
#pragma warning restore 1998

            public static Task<Container> GetAsync() => _instance.Value;
        }
    }
}
