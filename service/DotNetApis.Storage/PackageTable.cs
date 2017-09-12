using System;
using System.Globalization;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Nuget;
using Microsoft.WindowsAzure.Storage.Table;

namespace DotNetApis.Storage
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
        /// <param name="idver">The id and version of the package.</param>
        Task<PackageTableRecord?> TryGetRecordAsync(NugetPackageIdVersion idver);

        /// <summary>
        /// Writes an entry in the table, overwriting any existing entry for the package.
        /// </summary>
        /// <param name="idver">The id and version of the package.</param>
        /// <param name="record">The record for the package. The blob referenced by <c>record.BlobPath</c> must already exist.</param>
        Task SetRecordAsync(NugetPackageIdVersion idver, PackageTableRecord record);
    }

    public sealed class AzurePackageTable : IPackageTable
    {
        /// <summary>
        /// The version of the table schema
        /// </summary>
        private const int Version = 0;
        private readonly CloudTable _table;

        public static string TableName { get; } = "package" + Version;

        public AzurePackageTable(CloudTable table)
        {
            _table = table;
        }

        public async Task<PackageTableRecord?> TryGetRecordAsync(NugetPackageIdVersion idver)
        {
            var entity = await Entity.FindOrDefaultAsync(_table, idver).ConfigureAwait(false);
            if (entity == null)
                return null;
            return new PackageTableRecord { Path = entity.Path, Published = entity.Published };
        }

        public Task SetRecordAsync(NugetPackageIdVersion idver, PackageTableRecord record)
        {
            var entity = new Entity(_table, idver) { Path = record.Path, Published = record.Published };
            return entity.InsertOrReplaceAsync();
        }

        private sealed class Entity : TableEntityBase
        {
            public Entity(CloudTable table, NugetPackageIdVersion idver)
                : base(table, ToPartitionKey(idver), ToRowKey(idver))
            {
            }

            private Entity(CloudTable table, DynamicTableEntity entity)
                : base(table, entity)
            {
            }

            private static string ToPartitionKey(NugetPackageIdVersion idver) => idver.PackageId;

            private static string ToRowKey(NugetPackageIdVersion idver) => idver.Version.ToString();

            /// <summary>
            /// Performs a point search in the Azure table. Returns <c>null</c> if the entity is not found.
            /// </summary>
            /// <param name="table">The table to search.</param>
            /// <param name="idver">The id/version of the entity to retrieve.</param>
            public static async Task<Entity> FindOrDefaultAsync(CloudTable table, NugetPackageIdVersion idver)
            {
                var entity = await table.FindOrDefaultAsync(ToPartitionKey(idver), ToRowKey(idver)).ConfigureAwait(false);
                return entity == null ? null : new Entity(table, entity);
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
