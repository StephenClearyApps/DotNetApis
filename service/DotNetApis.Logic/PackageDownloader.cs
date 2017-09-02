using System.Threading.Tasks;
using DotNetApis.Nuget;
using DotNetApis.Storage;
using Microsoft.Extensions.Logging;

namespace DotNetApis.Logic
{
    /// <summary>
    /// Downloads package from Nuget to our own package storage as necessary.
    /// </summary>
    public sealed class PackageDownloader
    {
        private readonly ILogger _logger;
        private readonly IPackageTable _packageTable;
        private readonly IPackageStorage _packageStorage;
        private readonly INugetRepository _nugetRepository;

        public PackageDownloader(ILogger logger, IPackageTable packageTable, IPackageStorage packageStorage, INugetRepository nugetRepository)
        {
            _logger = logger;
            _packageTable = packageTable;
            _packageStorage = packageStorage;
            _nugetRepository = nugetRepository;
        }

        /// <summary>
        /// Reads a package from our package storage, along with its external metadata.
        /// If the package is not found, then this method downloads the package from Nuget, saves it in our storage, and returns it.
        /// If the package was just downloaded, the returned package is the instance retrieved from NuGet.
        /// </summary>
        /// <param name="idver">The package to retrieve.</param>
        public async Task<NugetFullPackage> GetPackageAsync(NugetPackageIdVersion idver)
        {
            _logger.LogDebug("Retrieving package {idver}", idver);
            var record = await _packageTable.TryGetRecordAsync(idver).ConfigureAwait(false);
            if (record != null)
            {
                // Read it from local Azure storage.
                var package = await _packageStorage.LoadAsync(record.Value.Path).ConfigureAwait(false);
                return new NugetFullPackage(package, new NugetPackageExternalMetadata(record.Value.Published));
            }
            else
            {
                // Download it from NuGet.
                var package = _nugetRepository.DownloadPackage(idver);
                var published = package.ExternalMetadata.Published;

                // Save it in our own storage.
                var path = idver.ToString();
                await _packageStorage.SaveAsync(path, package.Package).ConfigureAwait(false);
                await _packageTable.SetRecordAsync(idver, new PackageTableRecord { Path = path, Published = published }).ConfigureAwait(false);
                return package;
            }
        }

        /// <summary>
        /// Searches Nuget for a match for the specified version range, and then reads that package from our package storage, along with its external metadata.
        /// Returns <c>null</c> if there is no matching package found.
        /// If the package is matched but not in our package store, then this method downloads the package from Nuget, saves it in our storage, and returns it.
        /// If the package was just downloaded, the returned package is the instance retrieved from NuGet.
        /// </summary>
        /// <param name="id">The package id.</param>
        /// <param name="versionRange">The version range the package must match.</param>
        public async Task<NugetFullPackage> TryGetPackageAsync(string id, NugetVersionRange versionRange)
        {
            var idver = _nugetRepository.TryLookupPackage(id, versionRange);
            if (idver == null)
                return null;
            return await GetPackageAsync(idver).ConfigureAwait(false);
        }
    }
}
