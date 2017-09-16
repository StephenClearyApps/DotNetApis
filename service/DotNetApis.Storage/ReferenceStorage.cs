using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNetApis.Common;
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
        /// Returns the paths of files within a specific folder. This includes .xml as well as .dll files.
        /// </summary>
        /// <param name="path">The path of the folder to search.</param>
        Task<List<string>> GetFilesAsync(string path);

        /// <summary>
        /// Retrieves an assembly file from reference storage.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        Task<MemoryStream> DownloadAsync(string path);
    }

    public sealed class AzureReferenceStorage : IReferenceStorage
    {
        private readonly CloudBlobContainer _container;

        public static string ContainerName { get; } = "reference";

        public AzureReferenceStorage(CloudBlobContainer container)
        {
            _container = container;
        }

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

        public async Task<MemoryStream> DownloadAsync(string path)
        {
            var result = new MemoryStream();
            await _container.GetBlockBlobReference(path).DownloadToStreamAsync(result).ConfigureAwait(false);
            result.Position = 0;
            return result;
        }
    }
}
