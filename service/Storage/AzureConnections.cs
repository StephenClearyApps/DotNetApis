using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Common;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace Storage
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
    }
}
