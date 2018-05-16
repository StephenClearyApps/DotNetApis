using System;
using System.Diagnostics;
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
        private readonly ILogger<AzurePackageStorage> _logger;
        private readonly CloudBlobContainer _container;

        public static string ContainerName { get; } = "package";

        public AzurePackageStorage(ILoggerFactory loggerFactory, CloudBlobContainer container)
        {
            _logger = loggerFactory.CreateLogger<AzurePackageStorage>();
            _container = container;
        }

        public async Task<NugetPackage> LoadAsync(string path)
        {
            _logger.LoadingNupkg(path);
            var stopwatch = Stopwatch.StartNew();
            var stream = new MemoryStream();
            await _container.GetBlockBlobReference(path).DownloadToStreamAsync(stream).ConfigureAwait(false);
            stream.Position = 0;
            var result = new NugetPackage(stream);
            _logger.LoadedNupkg(path, stopwatch.Elapsed);
            return result;
        }

        public async Task SaveAsync(string path, NugetPackage package)
        {
            _logger.SavingNupkg(package, path);
            var stopwatch = Stopwatch.StartNew();
            await _container.GetBlockBlobReference(path).UploadFromStreamAsync(package.Stream).ConfigureAwait(false);
            _logger.SavedNupkg(package, path, stopwatch.Elapsed);
        }
    }

    internal static partial class Logging
    {
        public static void LoadingNupkg(this ILogger<AzurePackageStorage> logger, string path) =>
            Logger.Log(logger, 1, LogLevel.Debug, "Loading nupkg from blob {path}", path, null);

        public static void LoadedNupkg(this ILogger<AzurePackageStorage> logger, string path, TimeSpan elapsed) =>
            Logger.Log(logger, 2, LogLevel.Debug, "Successfully loaded nupkg from blob {path} in {elapsed}", path, elapsed, null);

        public static void SavingNupkg(this ILogger<AzurePackageStorage> logger, NugetPackage package, string path) =>
            Logger.Log(logger, 3, LogLevel.Debug, "Saving nupkg {package} to blob {path}", package, path, null);

        public static void SavedNupkg(this ILogger<AzurePackageStorage> logger, NugetPackage package, string path, TimeSpan elapsed) =>
            Logger.Log(logger, 4, LogLevel.Debug, "Successfully saved nupkg {package} to blob {path} in {elapsed}", package, path, elapsed, null);
    }
}
