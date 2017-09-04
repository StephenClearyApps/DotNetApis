using System.Threading.Tasks;
using DotNetApis.Nuget;
using DotNetApis.Structure;
using Microsoft.WindowsAzure.Storage.Table;

namespace DotNetApis.Storage
{
    /// <summary>
    /// Represents the "packagejson" table in Table storage.
    /// </summary>
    public interface IPackageJsonTable
    {
        /// <summary>
        /// Looks up a package in the table, and returns the path to the blob representing the json for that package. Returns <c>null</c> if the package is not in the table.
        /// </summary>
        /// <param name="idVer">The id and version of the package.</param>
        /// <param name="target">The target context.</param>
        Task<string> TryGetBlobPathAsync(NugetPackageIdVersion idVer, PlatformTarget target);

        /// <summary>
        /// Writes an entry in the table, overwriting any existing entry for the package.
        /// </summary>
        /// <param name="idVer">The id and version of the package.</param>
        /// <param name="target">The target context.</param>
        /// <param name="blobPath">The path to the blob representing the JSON for that dll. The blob must already exist.</param>
        Task SetBlobPathAsync(NugetPackageIdVersion idVer, PlatformTarget target, string blobPath);
    }

    public sealed class AzurePackageJsonTable : IPackageJsonTable
    {
        /// <summary>
        /// The version of the table schema
        /// </summary>
        private const int Version = 0;
        private readonly CloudTable _table;

        public AzurePackageJsonTable(AzureConnections connections)
        {
            _table = GetTable(connections);
        }

        private static CloudTable GetTable(AzureConnections connections) => connections.CloudTableClient.GetTableReference("packagejson" + Version + "x" + JsonFactory.Version);

        public static Task InitializeAsync() => GetTable(new AzureConnections()).CreateIfNotExistsAsync();

        public async Task<string> TryGetBlobPathAsync(NugetPackageIdVersion idVer, PlatformTarget target)
        {
            var entity = await Entity.FindOrDefaultAsync(_table, idVer, target).ConfigureAwait(false);
            return entity?.BlobPath;
        }

        public Task SetBlobPathAsync(NugetPackageIdVersion idVer, PlatformTarget target, string blobPath)
        {
            var entity = new Entity(_table, idVer, target) { BlobPath = blobPath };
            return entity.InsertOrReplaceAsync();
        }

        private sealed class Entity : TableEntityBase
        {
            public Entity(CloudTable table, NugetPackageIdVersion idVer, PlatformTarget target)
                : base(table, ToPartitionKey(idVer), ToRowKey(target))
            {
            }

            private Entity(CloudTable table, DynamicTableEntity entity)
                : base(table, entity)
            {
            }

            private static string ToPartitionKey(NugetPackageIdVersion idVer) => idVer.PackageId + "|" + idVer.Version;

            private static string ToRowKey(PlatformTarget target) => target.ToString();

            public static async Task<Entity> FindOrDefaultAsync(CloudTable table, NugetPackageIdVersion idVer, PlatformTarget target)
            {
                var entity = await table.FindOrDefaultAsync(ToPartitionKey(idVer), ToRowKey(target)).ConfigureAwait(false);
                return entity == null ? null : new Entity(table, entity);
            }

            public string BlobPath
            {
                get => Get("p", null);
                set => Set("p", value);
            }
        }
    }
}
