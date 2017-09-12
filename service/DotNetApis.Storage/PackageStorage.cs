using System.IO;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Nuget;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DotNetApis.Storage
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

        public static string ContainerName { get; } = "package";

        public AzurePackageStorage(ILogger logger, InstanceOf<CloudBlobContainer>.For<AzurePackageStorage> container)
        {
            _logger = logger;
            _container = container.Value;
        }

        public async Task<NugetPackage> LoadAsync(string path)
        {
            _logger.LogDebug("Loading nupkg from blob {path}", path);
            var stream = new MemoryStream();
            await _container.GetBlockBlobReference(path).DownloadToStreamAsync(stream).ConfigureAwait(false);
            stream.Position = 0;
            var result = new NugetPackage(stream);
            _logger.LogDebug("Successfully loaded nupkg from blob {path}", path);
            return result;
        }

        public async Task SaveAsync(string path, NugetPackage package)
        {
            _logger.LogDebug("Saving nupkg {package} to blob {path}", package, path);
            await _container.GetBlockBlobReference(path).UploadFromStreamAsync(package.Stream).ConfigureAwait(false);
            _logger.LogDebug("Successfully saved nupkg `{package}` to blob `{path}`", package, path);
        }
    }
}
