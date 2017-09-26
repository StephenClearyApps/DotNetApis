using System;
using System.Threading.Tasks;
using DotNetApis.Nuget;
using DotNetApis.Structure;
using Microsoft.WindowsAzure.Storage.Table;

namespace DotNetApis.Storage
{
    /// <summary>
    /// The status of a background operation.
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// The operation has been requested and has possibly started.
        /// </summary>
        Requested,

        /// <summary>
        /// The operation has succeeded.
        /// </summary>
        Succeeded,

        /// <summary>
        /// The operation has failed.
        /// </summary>
        Failed,
    }

    /// <summary>
    /// Represents the "status" table in Table storage.
    /// </summary>
    public interface IStatusTable
    {
        /// <summary>
        /// Looks up a package in the status table, and returns the record for that package. Returns <c>null</c> if the package is not in the table.
        /// </summary>
        /// <param name="idver">The id and version of the package.</param>
        /// <param name="target">The target of the package.</param>
        /// <param name="timestamp">The timestamp of the original documentation request.</param>
        Task<(Status Status, Uri LogUri, Uri JsonUri)?> TryGetStatusAsync(NugetPackageIdVersion idver, PlatformTarget target, DateTimeOffset timestamp);

        /// <summary>
        /// Writes an entity to the status table, overwriting any existing entity.
        /// </summary>
        /// <param name="idver">The id and version of the package.</param>
        /// <param name="target">The target of the package.</param>
        /// <param name="timestamp">The timestamp of the original documentation request.</param>
        /// <param name="status">The result of the documentation request.</param>
        /// <param name="logUri">The URI of the detailed log of the documentation generation.</param>
        /// <param name="jsonUri">The URI of the JSON result. May be <c>null</c>.</param>
        Task WriteStatusAsync(NugetPackageIdVersion idver, PlatformTarget target, DateTimeOffset timestamp, Status status, Uri logUri, Uri jsonUri);
    }

    public sealed class AzureStatusTable : IStatusTable
    {
        /// <summary>
        /// The version of the table schema
        /// </summary>
        private const int Version = 0;

        private readonly CloudTableClient _cloudTableClient;

        public AzureStatusTable(CloudTableClient cloudTableClient)
        {
            _cloudTableClient = cloudTableClient;
        }

        private CloudTable GetTable(DateTimeOffset timestamp)
        {
            return _cloudTableClient.GetTableReference("status" + Version + "x" + timestamp.ToString("yyyyMMdd"));
        }

        public async Task<(Status Status, Uri LogUri, Uri JsonUri)?> TryGetStatusAsync(NugetPackageIdVersion idver, PlatformTarget target, DateTimeOffset timestamp)
        {
            var table = GetTable(timestamp);
            if (!await table.ExistsAsync().ConfigureAwait(false))
                return null;
            var entity = await Entity.FindOrDefaultAsync(table, idver, target).ConfigureAwait(false);
            return (entity.Status, entity.LogUri, entity.JsonUri);
        }

        public async Task WriteStatusAsync(NugetPackageIdVersion idver, PlatformTarget target, DateTimeOffset timestamp, Status status, Uri logUri, Uri jsonUri)
        {
            var table = GetTable(timestamp);
            await table.CreateIfNotExistsAsync().ConfigureAwait(false);
            var entity = new Entity(table, idver, target)
            {
                Status = status,
                LogUri = logUri,
                JsonUri = jsonUri,
            };
            await entity.InsertOrReplaceAsync().ConfigureAwait(false);
        }

        private sealed class Entity : TableEntityBase
        {
            public Entity(CloudTable table, NugetPackageIdVersion idver, PlatformTarget target)
                : base(table, ToPartitionKey(idver), ToRowKey(idver, target))
            {
            }

            private Entity(CloudTable table, DynamicTableEntity entity)
                : base(table, entity)
            {
            }

            private static string ToPartitionKey(NugetPackageIdVersion idver) => JsonFactory.Version + "|" + idver.PackageId;

            private static string ToRowKey(NugetPackageIdVersion idver, PlatformTarget target) => idver.Version + "|" + target;

            /// <summary>
            /// Performs a point search in the Azure table. Returns <c>null</c> if the entity is not found.
            /// </summary>
            /// <param name="table">The table to search.</param>
            /// <param name="idver">The id/version of the entity to retrieve.</param>
            /// <param name="target">The target of the entity to retrieve.</param>
            public static async Task<Entity> FindOrDefaultAsync(CloudTable table, NugetPackageIdVersion idver, PlatformTarget target)
            {
                var entity = await table.FindOrDefaultAsync(ToPartitionKey(idver), ToRowKey(idver, target)).ConfigureAwait(false);
                return entity == null ? null : new Entity(table, entity);
            }

            public Status Status
            {
                get => (Status)Get("s", 0);
                set => Set("s", (int)value);
            }

            public Uri LogUri
            {
                get
                {
                    var stringValue = Get("l", null);
                    return stringValue == null ? null : new Uri(stringValue);
                }
                set => Set("l", value?.ToString());
            }

            public Uri JsonUri
            {
                get
                {
                    var stringValue = Get("j", null);
                    return stringValue == null ? null : new Uri(stringValue);
                }
                set => Set("j", value?.ToString());
            }
        }
    }
}
