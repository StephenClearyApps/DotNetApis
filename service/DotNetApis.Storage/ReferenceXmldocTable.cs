using System;
using System.Globalization;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Nuget;
using Microsoft.WindowsAzure.Storage.Table;

namespace DotNetApis.Storage
{
    public struct ReferenceXmldocTableRecord
    {
        /// <summary>
        /// The equivalent DNA identifier for the specified Xmldoc identifier.
        /// </summary>
        public string DnaId { get; set; }

        /// <summary>
        /// The friendly name for the entity specified by the Xmldoc identifier.
        /// </summary>
        public FriendlyName FriendlyName { get; set; }
    }

    /// <summary>
    /// Represents the "referencexmldoc" table in Table storage.
    /// </summary>
    public interface IReferenceXmldocTable
    {
        /// <summary>
        /// Looks up xmldoc in the table, and returns the record for that xmldoc. Returns <c>null</c> if the xmldoc is not in the table.
        /// </summary>
        Task<ReferenceXmldocTableRecord?> TryGetRecordAsync(PlatformTarget framework, string xmldoc);

        /// <summary>
        /// Looks up xmldoc in the table, and returns the record for that xmldoc. Returns <c>null</c> if the xmldoc is not in the table.
        /// </summary>
        ReferenceXmldocTableRecord? TryGetRecord(PlatformTarget framework, string xmldoc);

        /// <summary>
        /// Writes an entry in the table, overwriting any existing entry for the xmldoc.
        /// </summary>
        Task SetRecordAsync(PlatformTarget framework, string xmldoc, ReferenceXmldocTableRecord record);
    }

    public sealed class AzureReferenceXmldocTable : IReferenceXmldocTable
    {
        /// <summary>
        /// The version of the table schema
        /// </summary>
        private const int Version = 0;
        private readonly CloudTable _table;
        
        public AzureReferenceXmldocTable(AzureConnections connections)
        {
            _table = GetTable(connections);
        }

        private static CloudTable GetTable(AzureConnections connections) => connections.CloudTableClient.GetTableReference("referencexmldoc" + Version);

        public static Task InitializeAsync(AzureConnections connections) => GetTable(connections).CreateIfNotExistsAsync();

        private async Task<ReferenceXmldocTableRecord?> TryGetRecordAsync(PlatformTarget framework, string xmldoc, bool sync)
        {
            var entity = await Entity.FindOrDefaultAsync(_table, framework, xmldoc, sync).ConfigureAwait(false);
            if (entity == null)
                return null;
            return new ReferenceXmldocTableRecord
            {
                DnaId = entity.DnaId,
                FriendlyName = new FriendlyName(entity.SimpleName, entity.QualifiedName, entity.FullyQualifiedName),
            };
        }

        public Task<ReferenceXmldocTableRecord?> TryGetRecordAsync(PlatformTarget framework, string xmldoc) => TryGetRecordAsync(framework, xmldoc, sync: false);

        public ReferenceXmldocTableRecord? TryGetRecord(PlatformTarget framework, string xmldoc) => TryGetRecordAsync(framework, xmldoc, sync: true).GetAwaiter().GetResult();
        
        public Task SetRecordAsync(PlatformTarget framework, string xmldoc, ReferenceXmldocTableRecord record)
        {
            var entity = new Entity(_table, framework, xmldoc)
            {
                DnaId = record.DnaId,
                SimpleName = record.FriendlyName.SimpleName,
                QualifiedName = record.FriendlyName.QualifiedName,
                FullyQualifiedName = record.FriendlyName.FullyQualifiedName,
            };
            return entity.InsertOrReplaceAsync();
        }

        private sealed class Entity : TableEntityBase
        {
            public Entity(CloudTable table, PlatformTarget framework, string xmldoc)
                : base(table, ToPartitionKey(framework), ToRowKey(xmldoc))
            {
            }

            private Entity(CloudTable table, DynamicTableEntity entity)
                : base(table, entity)
            {
            }

            private static string ToPartitionKey(PlatformTarget framework) => framework.ToString();

            private static string ToRowKey(string xmldoc) => StorageUtility.HashString(xmldoc);

            /// <summary>
            /// Performs a point search in the Azure table. Returns <c>null</c> if the entity is not found.
            /// </summary>
            public static async Task<Entity> FindOrDefaultAsync(CloudTable table, PlatformTarget framework, string xmldoc, bool sync)
            {
                var partitionKey = ToPartitionKey(framework);
                var rowKey = ToRowKey(xmldoc);
                var entity = sync ? table.FindOrDefault(partitionKey, rowKey) : await table.FindOrDefaultAsync(partitionKey, rowKey).ConfigureAwait(false);
                return entity == null ? null : new Entity(table, entity);
            }

            public string DnaId
            {
                get => Get("i", null);
                set => Set("i", value);
            }

            public string SimpleName
            {
                get => Get("n", null);
                set => Set("n", value);
            }

            public string QualifiedName
            {
                get => Get("q", null);
                set => Set("q", value);
            }

            public string FullyQualifiedName
            {
                get => Get("f", null);
                set => Set("f", value);
            }

            public DynamicTableEntity UnderlyingEntity => Entity;
        }
    }
}
