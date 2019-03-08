using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Logic;
using DotNetApis.SimpleInjector;
using DotNetApis.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using Microsoft.WindowsAzure.Storage.Table;

namespace FunctionApp.CompositionRoot
{
    /// <summary>
    /// Singletons shared between containers.
    /// </summary>
    public static class Singletons
    {
        public static readonly IAsyncSingleton<ReferenceAssemblies> ReferenceAssembliesInstance = Singleton.Create(async () => await ReferenceAssemblies.CreateAsync(await ReferenceStorageInstance));
        public static readonly IAsyncSingleton<IReferenceStorage> ReferenceStorageInstance = Singleton.Create(async () => new AzureReferenceStorage((await ReferenceStorageCloudBlobContainer).Value) as IReferenceStorage);

        public static readonly IAsyncSingleton<InstanceOf<CloudBlobContainer>.For<AzureReferenceStorage>> ReferenceStorageCloudBlobContainer = CreateContainerAsync<AzureReferenceStorage>(AzureReferenceStorage.ContainerName, publicAccess: false);
        public static readonly IAsyncSingleton<InstanceOf<CloudTable>.For<AzureReferenceXmldocTable>> ReferenceXmldocTableCloudTable = CreateTableAsync<AzureReferenceXmldocTable>(AzureReferenceXmldocTable.TableName);
        public static readonly IAsyncSingleton<InstanceOf<CloudBlobContainer>.For<AzurePackageJsonStorage>> PackageJsonStorageCloudBlobContainer = CreateContainerAsync<AzurePackageJsonStorage>(AzurePackageJsonStorage.ContainerName, publicAccess: true);
        public static readonly IAsyncSingleton<InstanceOf<CloudTable>.For<AzurePackageJsonTable>> PackageJsonTableCloudTable = CreateTableAsync<AzurePackageJsonTable>(AzurePackageJsonTable.TableName);
        public static readonly IAsyncSingleton<InstanceOf<CloudTable>.For<AzurePackageTable>> PackageTableCloudTable = CreateTableAsync<AzurePackageTable>(AzurePackageTable.TableName);
        public static readonly IAsyncSingleton<InstanceOf<CloudBlobContainer>.For<AzurePackageStorage>> PackageStorageCloudBlobContainer = CreateContainerAsync<AzurePackageStorage>(AzurePackageStorage.ContainerName, publicAccess: false);

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

        public static readonly ISingleton<CloudTableClient> CloudTableClientInstance = Singleton.Create(() => CloudStorageAccountInstance.Value.CreateCloudTableClient());

        public static readonly IAsyncSingleton<CloudBlobClient> CloudBlobClientInstance = Singleton.Create(async () =>
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

        public static readonly ISingleton<CloudStorageAccount> CloudStorageAccountInstance = Singleton.Create(() =>
        {
            var connectionString = AmbientContext.ConfigurationRoot.GetValue<string>("StorageConnectionString", null);
            if (connectionString == null)
                throw new InvalidOperationException("No StorageConnectionString setting found; update your copy of local.settings.json to include this value.");
            return CloudStorageAccount.Parse(connectionString);
        });
    }
}
