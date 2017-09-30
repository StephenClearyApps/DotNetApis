using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Logic;
using DotNetApis.Nuget;
using DotNetApis.SimpleInjector;
using DotNetApis.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using Microsoft.WindowsAzure.Storage.Table;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace FunctionApp
{
    public static class CompositionRoot
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

        private static readonly IAsyncSingleton<ReferenceAssemblies> ReferenceAssembliesInstance = Singleton.Create(async () => await ReferenceAssemblies.CreateAsync(await ReferenceStorageInstance));
        private static readonly IAsyncSingleton<IReferenceStorage> ReferenceStorageInstance = Singleton.Create(async () => new AzureReferenceStorage((await ReferenceStorageCloudBlobContainer).Value) as IReferenceStorage);

        private static readonly IAsyncSingleton<InstanceOf<CloudBlobContainer>.For<AzureReferenceStorage>> ReferenceStorageCloudBlobContainer = CreateContainerAsync<AzureReferenceStorage>(AzureReferenceStorage.ContainerName, publicAccess: false);
        private static readonly IAsyncSingleton<InstanceOf<CloudTable>.For<AzureReferenceXmldocTable>> ReferenceXmldocTableCloudTable = CreateTableAsync<AzureReferenceXmldocTable>(AzureReferenceXmldocTable.TableName);
        private static readonly IAsyncSingleton<InstanceOf<CloudBlobContainer>.For<AzurePackageJsonStorage>> PackageJsonStorageCloudBlobContainer = CreateContainerAsync<AzurePackageJsonStorage>(AzurePackageJsonStorage.ContainerName, publicAccess: true);
        private static readonly IAsyncSingleton<InstanceOf<CloudTable>.For<AzurePackageJsonTable>> PackageJsonTableCloudTable = CreateTableAsync<AzurePackageJsonTable>(AzurePackageJsonTable.TableName);
        private static readonly IAsyncSingleton<InstanceOf<CloudTable>.For<AzurePackageTable>> PackageTableCloudTable = CreateTableAsync<AzurePackageTable>(AzurePackageTable.TableName);
        private static readonly IAsyncSingleton<InstanceOf<CloudBlobContainer>.For<AzurePackageStorage>> PackageStorageCloudBlobContainer = CreateContainerAsync<AzurePackageStorage>(AzurePackageStorage.ContainerName, publicAccess: false);

        // Special-purpose factory methods to reduce code duplication.
        private static IAsyncSingleton<InstanceOf<CloudBlobContainer>.For<T>> CreateContainerAsync<T>(string name, bool publicAccess) => Singleton.Create(async () =>
        {
            var client = await CloudBlobClientInstance;
            var result = client.GetContainerReference(name);
            await result.CreateIfNotExistsAsync();
            if (publicAccess)
            {
                var permissions = await result.GetPermissionsAsync();
                if (permissions.PublicAccess != BlobContainerPublicAccessType.Blob)
                {
                    permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                    await result.SetPermissionsAsync(permissions);
                }
            }
            return new InstanceOf<CloudBlobContainer>.For<T>(result);
        });
        private static IAsyncSingleton<InstanceOf<CloudTable>.For<T>> CreateTableAsync<T>(string name) => Singleton.Create(async () =>
        {
            var result = CloudTableClientInstance.Value.GetTableReference(name);
            await result.CreateIfNotExistsAsync();
            return new InstanceOf<CloudTable>.For<T>(result);
        });

        private static readonly ISingleton<CloudTableClient> CloudTableClientInstance = Singleton.Create(() => CloudStorageAccountInstance.Value.CreateCloudTableClient());

        private static readonly IAsyncSingleton<CloudBlobClient> CloudBlobClientInstance = Singleton.Create(async () =>
        {
            var result = CloudStorageAccountInstance.Value.CreateCloudBlobClient();
            var properties = await result.GetServicePropertiesAsync();
            properties.Cors = new CorsProperties();
            properties.Cors.CorsRules.Add(new CorsRule
            {
                AllowedHeaders = { "*" },
                AllowedMethods = CorsHttpMethods.Get,
                AllowedOrigins = { "*" },
                ExposedHeaders = { "*" },
                MaxAgeInSeconds = 31536000,
            });
            await result.SetServicePropertiesAsync(properties);
            return result;
        });

        private static readonly ISingleton<CloudStorageAccount> CloudStorageAccountInstance = Singleton.Create(() =>
        {
            var connectionString = Config.GetSetting("StorageConnectionString");
            if (connectionString == null)
                throw new InvalidOperationException("No StorageConnectionString setting found; update your copy of local.settings.json to include this value.");
            return CloudStorageAccount.Parse(connectionString);
        });
    }
}
