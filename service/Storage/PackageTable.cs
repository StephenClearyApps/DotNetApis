using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Nuget;

namespace Storage
{
    public struct PackageTableRecord
    {
        /// <summary>
        /// The path of this nupkg in the package storage.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The publication date of this package.
        /// </summary>
        public DateTimeOffset Published { get; set; }
    }

    /// <summary>
    /// Represents the "package" table in Table storage.
    /// </summary>
    public interface IPackageTable
    {
        /// <summary>
        /// Looks up a package in the table, and returns the record for that package. Returns <c>null</c> if the package is not in the table.
        /// </summary>
        /// <param name="idVer">The id and version of the package.</param>
        Task<PackageTableRecord?> TryGetRecordAsync(NugetPackageIdVersion idVer);

        /// <summary>
        /// Writes an entry in the table, overwriting any existing entry for the package.
        /// </summary>
        /// <param name="idVer">The id and version of the package.</param>
        /// <param name="record">The record for the package. The blob referenced by <c>record.BlobPath</c> must already exist.</param>
        Task SetRecordAsync(NugetPackageIdVersion idVer, PackageTableRecord record);
    }

    public sealed class AzurePackageTable : IPackageTable
    {
        /// <summary>
        /// The version of the table schema
        /// </summary>
        private const int Version = 0;
        private readonly CloudTable _table;

        public AzurePackageTable(AzureConnections connections)
        {
            _table = GetTable(connections);
        }

        private static CloudTable GetTable(AzureConnections connections)
        {
            return connections.CloudTableClient.GetTableReference("package" + Version);
        }

        public static Task InitializeAsync() => GetTable(new AzureConnections()).CreateIfNotExistsAsync();

        public async Task<PackageTableRecord?> TryGetRecordAsync(NugetPackageIdVersion idVer)
        {
            var entity = await Entity.FindOrDefaultAsync(_table, idVer).ConfigureAwait(false);
            if (entity == null)
                return null;
            return new PackageTableRecord { Path = entity.Path, Published = entity.Published };
        }

        public Task SetRecordAsync(NugetPackageIdVersion idVer, PackageTableRecord record)
        {
            var entity = new Entity(_table, idVer) { Path = record.Path, Published = record.Published };
            return entity.InsertOrReplaceAsync();
        }

        private sealed class Entity : TableEntityBase
        {
            public Entity(CloudTable table, NugetPackageIdVersion idVer)
                : base(table, ToPartitionKey(idVer), ToRowKey(idVer))
            {
            }

            private Entity(CloudTable table, DynamicTableEntity entity)
                : base(table, entity)
            {
            }

            private static string ToPartitionKey(NugetPackageIdVersion idVer) => idVer.PackageId;

            private static string ToRowKey(NugetPackageIdVersion idVer) => idVer.Version.ToString();

            /// <summary>
            /// Performs a point search in the Azure table. Returns <c>null</c> if the entity is not found.
            /// </summary>
            /// <param name="table">The table to search.</param>
            /// <param name="idVer">The id/version of the entity to retrieve.</param>
            public static async Task<Entity> FindOrDefaultAsync(CloudTable table, NugetPackageIdVersion idVer)
            {
                var entity = await table.FindOrDefaultAsync(ToPartitionKey(idVer), ToRowKey(idVer)).ConfigureAwait(false);
                if (entity == null)
                    return null;
                return new Entity(table, entity);
            }

            public string Path
            {
                get => Get("p", null);
                set => Set("p", value);
            }

            public DateTimeOffset Published
            {
                get => DateTimeOffset.ParseExact(Get("d", null), "o", null, DateTimeStyles.RoundtripKind);
                set => Set("d", value.ToString("o"));
            }
        }
    }
}
