using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Logic;
using DotNetApis.Nuget;
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
        public static async Task<Container> GetContainerForDocumentationFunctionAsync(ILogger log, TraceWriter writer, bool requestIsLocal)
        {
            var inMemoryLogger = new InMemoryLogger();
            var logger = new CompositeLogger(Enumerables.Return(inMemoryLogger, log, requestIsLocal ? new TraceWriterLogger(writer) : null));
            try
            {
                // Singletons
                var referenceStorage = await ReferenceStorageInstance;
                var referenceAssemblies = await ReferenceAssembliesInstance;

                // Scoped
                var packageStorage = new AzurePackageStorage(logger, await PackageStorageCloudBlobContainer);
                var packageTable = new AzurePackageTable(await PackageTableCloudTable);
                var packageJsonTable = new AzurePackageJsonTable(await PackageJsonTableCloudTable);
                var packageJsonStorage = new AzurePackageJsonStorage(logger, await PackageJsonStorageCloudBlobContainer);
                var referenceXmldocTable = new AzureReferenceXmldocTable(await ReferenceXmldocTableCloudTable);

                var container = new Container();
                container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
                container.Options.DefaultLifestyle = Lifestyle.Scoped;
                container.RegisterSingleton<IReferenceStorage>(referenceStorage);
                container.RegisterSingleton(referenceAssemblies);
                container.Register(() => inMemoryLogger);
                container.Register<ILogger>(() => logger);
                container.Register<INugetRepository, NugetRepository>();
                container.Register<ILogStorage, AzureLogStorage>();
                container.Register<IStatusTable, AzureStatusTable>();
                container.Register<IPackageStorage>(() => packageStorage);
                container.Register<IPackageTable>(() => packageTable);
                container.Register<IPackageJsonTable>(() => packageJsonTable);
                container.Register<IPackageJsonStorage>(() => packageJsonStorage);
                container.Register<IReferenceXmldocTable>(() => referenceXmldocTable);
                container.Verify();
                return container;
            }
            catch (Exception ex)
            {
                logger.LogCritical(0, ex, "Failed to create container composition root");
                throw;
            }
        }

        private static readonly AsyncLazy<ReferenceAssemblies> ReferenceAssembliesInstance = CreateAsync(async () => await ReferenceAssemblies.CreateAsync(await ReferenceStorageInstance));
        private static readonly AsyncLazy<AzureReferenceStorage> ReferenceStorageInstance = CreateAsync(async () => new AzureReferenceStorage(await ReferenceStorageCloudBlobContainer));

        private static readonly AsyncLazy<CloudBlobContainer> ReferenceStorageCloudBlobContainer = CreateContainerAsync(AzureReferenceStorage.ContainerName);
        private static readonly AsyncLazy<CloudTable> ReferenceXmldocTableCloudTable = CreateTableAsync(AzureReferenceXmldocTable.TableName);
        private static readonly AsyncLazy<CloudBlobContainer> PackageJsonStorageCloudBlobContainer = CreateContainerAsync(AzurePackageJsonStorage.ContainerName);
        private static readonly AsyncLazy<CloudTable> PackageJsonTableCloudTable = CreateTableAsync(AzurePackageJsonTable.TableName);
        private static readonly AsyncLazy<CloudTable> PackageTableCloudTable = CreateTableAsync(AzurePackageTable.TableName);
        private static readonly AsyncLazy<CloudBlobContainer> PackageStorageCloudBlobContainer = CreateContainerAsync(AzurePackageStorage.ContainerName);

        // Special-purpose factory methods to reduce code duplication.
        private static AsyncLazy<CloudBlobContainer> CreateContainerAsync(string name) => CreateAsync(async () =>
        {
            var client = await CloudBlobClientInstance;
            var result = client.GetContainerReference(name);
            await result.CreateIfNotExistsAsync();
            return result;
        });
        private static AsyncLazy<CloudTable> CreateTableAsync(string name) => CreateAsync(async () =>
        {
            var result = CloudTableClientInstance.Value.GetTableReference(name);
            await result.CreateIfNotExistsAsync();
            return result;
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
