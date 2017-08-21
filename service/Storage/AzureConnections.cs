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
            CloudStorageAccount = CloudStorageAccount.Parse(Config.GetSetting("StorageConnectionString"));
            CloudBlobClient = CloudStorageAccount.CreateCloudBlobClient();
        }

        public CloudStorageAccount CloudStorageAccount { get; }
        public CloudBlobClient CloudBlobClient { get; }
    }
}
