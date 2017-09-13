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
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using Microsoft.WindowsAzure.Storage.Table;
using Nito.AsyncEx;
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

        private static readonly AsyncLazy<Container> Container = CreateAsync(async () =>
        {
            var singletons = await AsyncTupleHelpers.WhenAll(ReferenceStorageInstance.Task, ReferenceAssembliesInstance.Task, PackageStorageCloudBlobContainer.Task,
                PackageTableCloudTable.Task, PackageJsonTableCloudTable.Task, PackageJsonStorageCloudBlobContainer.Task, ReferenceXmldocTableCloudTable.Task);

            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            container.Options.DefaultLifestyle = Lifestyle.Scoped;
            container.UseAutomaticInstanceOf();
            container.RegisterSingletons(singletons);
            container.Register<ILogger, AsyncLocalLogger>();
            container.Register<INugetRepository, NugetRepository>();
            container.Register<ILogStorage, AzureLogStorage>();
            container.Register<IStatusTable, AzureStatusTable>();
            container.Register<IPackageStorage, AzurePackageStorage>();
            container.Register<IPackageTable, AzurePackageTable>();
            container.Register<IPackageJsonTable, AzurePackageJsonTable>();
            container.Register<IPackageJsonStorage, AzurePackageJsonStorage>();
            container.Register<IReferenceXmldocTable, AzureReferenceXmldocTable>();
            container.Register<DocRequestHandler>();
            container.Verify();
            return container;
        });

        private static readonly AsyncLazy<ReferenceAssemblies> ReferenceAssembliesInstance = CreateAsync(async () => await ReferenceAssemblies.CreateAsync(await ReferenceStorageInstance));
        private static readonly AsyncLazy<IReferenceStorage> ReferenceStorageInstance = CreateAsync(async () => new AzureReferenceStorage((await ReferenceStorageCloudBlobContainer).Value) as IReferenceStorage);

        private static readonly AsyncLazy<InstanceOf<CloudBlobContainer>.For<AzureReferenceStorage>> ReferenceStorageCloudBlobContainer = CreateContainerAsync<AzureReferenceStorage>(AzureReferenceStorage.ContainerName);
        private static readonly AsyncLazy<InstanceOf<CloudTable>.For<AzureReferenceXmldocTable>> ReferenceXmldocTableCloudTable = CreateTableAsync<AzureReferenceXmldocTable>(AzureReferenceXmldocTable.TableName);
        private static readonly AsyncLazy<InstanceOf<CloudBlobContainer>.For<AzurePackageJsonStorage>> PackageJsonStorageCloudBlobContainer = CreateContainerAsync<AzurePackageJsonStorage>(AzurePackageJsonStorage.ContainerName);
        private static readonly AsyncLazy<InstanceOf<CloudTable>.For<AzurePackageJsonTable>> PackageJsonTableCloudTable = CreateTableAsync<AzurePackageJsonTable>(AzurePackageJsonTable.TableName);
        private static readonly AsyncLazy<InstanceOf<CloudTable>.For<AzurePackageTable>> PackageTableCloudTable = CreateTableAsync<AzurePackageTable>(AzurePackageTable.TableName);
        private static readonly AsyncLazy<InstanceOf<CloudBlobContainer>.For<AzurePackageStorage>> PackageStorageCloudBlobContainer = CreateContainerAsync<AzurePackageStorage>(AzurePackageStorage.ContainerName);

        // Special-purpose factory methods to reduce code duplication.
        private static AsyncLazy<InstanceOf<CloudBlobContainer>.For<T>> CreateContainerAsync<T>(string name) => CreateAsync(async () =>
        {
            var client = await CloudBlobClientInstance;
            var result = client.GetContainerReference(name);
            await result.CreateIfNotExistsAsync();
            return new InstanceOf<CloudBlobContainer>.For<T>(result);
        });
        private static AsyncLazy<InstanceOf<CloudTable>.For<T>> CreateTableAsync<T>(string name) => CreateAsync(async () =>
        {
            var result = CloudTableClientInstance.Value.GetTableReference(name);
            await result.CreateIfNotExistsAsync();
            return new InstanceOf<CloudTable>.For<T>(result);
        });

        private static readonly Lazy<CloudTableClient> CloudTableClientInstance = Create(() => CloudStorageAccountInstance.Value.CreateCloudTableClient());

        private static readonly AsyncLazy<CloudBlobClient> CloudBlobClientInstance = CreateAsync(async () =>
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

        private static readonly Lazy<CloudStorageAccount> CloudStorageAccountInstance = Create(() =>
        {
            var connectionString = Config.GetSetting("StorageConnectionString");
            if (connectionString == null)
                throw new InvalidOperationException("No StorageConnectionString setting found; update your copy of local.settings.json to include this value.");
            return CloudStorageAccount.Parse(connectionString);
        });

        // Factory methods with retries after failures.
        private static AsyncLazy<T> CreateAsync<T>(Func<Task<T>> factory) => new AsyncLazy<T>(factory, AsyncLazyFlags.ExecuteOnCallingThread | AsyncLazyFlags.RetryOnFailure);
        private static Lazy<T> Create<T>(Func<T> factory) => new Lazy<T>(factory, LazyThreadSafetyMode.PublicationOnly);
    }
}
