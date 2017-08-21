using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Common;
using Microsoft.WindowsAzure.Storage.Blob;

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
        }

        public CloudStorageAccount CloudStorageAccount { get; }
        public CloudBlobClient CloudBlobClient { get; }
    }
}
