using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.WindowsAzure.Storage.Blob;
using Nuget;

namespace Storage
{
    public interface IPackageStorage
    {
        /// <summary>
        /// Reads an entire package from storage.
        /// </summary>
        /// <param name="path">The path of the package to read.</param>
        Task<NugetPackage> LoadAsync(string path);

        /// <summary>
        /// Writes an entire package to storage, overwriting any existing package at that location.
        /// </summary>
        /// <param name="path">The path of the package to write.</param>
        /// <param name="package">The package to upload.</param>
        Task SaveAsync(string path, NugetPackage package);
    }

    public sealed class AzurePackageStorage : IPackageStorage
    {
        private readonly ILogger _logger;
        private readonly CloudBlobContainer _container;

        public AzurePackageStorage(ILogger logger, AzureConnections connections)
        {
            _logger = logger;
            _container = connections.CloudBlobClient.GetContainerReference("package");
        }

        public async Task<NugetPackage> LoadAsync(string path)
        {
            _logger.Trace($"Loading nupkg from blob `{path}`");
            var stream = new MemoryStream();
            await _container.GetBlockBlobReference(path).DownloadToStreamAsync(stream).ConfigureAwait(false);
            stream.Position = 0;
            var result = new NugetPackage(stream);
            _logger.Trace($"Successfully loaded nupkg from blob `{path}`");
            return result;
        }

        public async Task SaveAsync(string path, NugetPackage package)
        {
            _logger.Trace($"Saving nupkg `{package}` to blob `{path}`");
            await _container.GetBlockBlobReference(path).UploadFromStreamAsync(package.Stream).ConfigureAwait(false);
            _logger.Trace($"Successfully saved nupkg `{package}` to blob `{path}`");
        }
    }
}
