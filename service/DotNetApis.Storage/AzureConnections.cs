using System;
using System.Threading.Tasks;
using DotNetApis.Common;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using Microsoft.WindowsAzure.Storage.Table;

namespace DotNetApis.Storage
{
    public sealed class AzureConnections
    {
        public AzureConnections()
        {
            var connectionString = Config.GetSetting("StorageConnectionString");
            if (connectionString == null)
                throw new InvalidOperationException("No StorageConnectionString setting found; update your copy of local.settings.json to include this value.");
            CloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient = CloudStorageAccount.CreateCloudBlobClient();
            CloudTableClient = CloudStorageAccount.CreateCloudTableClient();
        }

        public CloudStorageAccount CloudStorageAccount { get; }
        public CloudBlobClient CloudBlobClient { get; }
        public CloudTableClient CloudTableClient { get; }

        public async Task InitializeAsync()
        {
            var properties = await CloudBlobClient.GetServicePropertiesAsync().ConfigureAwait(false);
            properties.Cors = new CorsProperties();
            properties.Cors.CorsRules.Add(new CorsRule
            {
                AllowedHeaders = { "*" },
                AllowedMethods = CorsHttpMethods.Get,
                AllowedOrigins = { "*" },
                ExposedHeaders = { "*" },
                MaxAgeInSeconds = 31536000,
            });
            await CloudBlobClient.SetServicePropertiesAsync(properties).ConfigureAwait(false);
        }
    }
}
