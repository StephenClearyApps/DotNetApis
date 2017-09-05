using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DotNetApis.Storage
{
    public interface IReferenceStorage
    {
        /// <summary>
        /// Returns the paths of reference folders available.
        /// </summary>
        Task<List<string>> GetFoldersAsync();

        /// <summary>
        /// Returns the paths of files within a specific folder.
        /// </summary>
        /// <param name="path">The path of the folder to search.</param>
        Task<List<string>> GetFilesAsync(string path);
    }

    public sealed class AzureReferenceStorage : IReferenceStorage
    {
        private readonly CloudBlobContainer _container;

        public AzureReferenceStorage(AzureConnections connections)
        {
            _container = GetContainer(connections);
        }

        private static CloudBlobContainer GetContainer(AzureConnections connections) => connections.CloudBlobClient.GetContainerReference("reference");

        public Task<List<string>> GetFoldersAsync() => ListAsync(null, results => results.OfType<CloudBlobDirectory>().Select(x => x.Prefix.TrimEnd('/')));

        public Task<List<string>> GetFilesAsync(string path) => ListAsync(path, results => results.OfType<CloudBlockBlob>().Select(x => x.Name));

        private async Task<List<string>> ListAsync(string path, Func<IEnumerable<IListBlobItem>, IEnumerable<string>> handler)
        {
            var result = new List<string>();
            BlobContinuationToken continuation = null;
            do
            {
                var segment = await _container.ListBlobsSegmentedAsync(path, false, BlobListingDetails.None, null, continuation, null, null).ConfigureAwait(false);
                continuation = segment.ContinuationToken;
                result.AddRange(handler(segment.Results));
            } while (continuation != null);
            return result;
        }

        public static Task InitializeAsync(AzureConnections connections) => GetContainer(connections).CreateIfNotExistsAsync();
    }
}
