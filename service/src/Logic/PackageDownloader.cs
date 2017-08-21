using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;
using Nuget;
using Storage;

namespace Logic
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
            _logger.Trace($"Retrieving package {idver}");
            var record = await _packageTable.TryGetRecordAsync(idver).ConfigureAwait(false);
            if (record != null)
            {
                // Read it from local Azure storage.
                _logger.Trace($"Package {idver} is in our own storage");
                var package = await _packageStorage.LoadAsync(record.Value.Path).ConfigureAwait(false);
                return new NugetFullPackage(package, new NugetPackageExternalMetadata(record.Value.Published));
            }
            else
            {
                // Download it from NuGet.
                _logger.Trace($"Downloading package {idver} from Nuget");
                var package = _nugetRepository.DownloadPackage(idver);
                var published = package.ExternalMetadata.Published;

                // Save it in our own storage.
                _logger.Trace($"Saving package {idver} locally");
                var path = idver.ToString();
                await _packageStorage.SaveAsync(path, package.Package).ConfigureAwait(false);
                await _packageTable.SetRecordAsync(idver, new PackageTableRecord { Path = path, Published = published }).ConfigureAwait(false);
                return package;
            }
        }
    }
}
