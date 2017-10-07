using System;
using System.Threading.Tasks;
using DotNetApis.Common;
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
    /// Represents the "packagejson" table in Table storage.
    /// </summary>
    public interface IPackageJsonTable
    {
        /// <summary>
        /// Looks up a package in the table, and returns the information for that package. Returns <c>null</c> if the package is not in the table.
        /// </summary>
        /// <param name="idVer">The id and version of the package.</param>
        /// <param name="target">The target context.</param>
        Task<(Status Status, Uri LogUri, Uri JsonUri)?> TryGetRecordAsync(NugetPackageIdVersion idVer, PlatformTarget target);

        /// <summary>
        /// Writes an entry in the table, overwriting any existing entry for the package.
        /// </summary>
        /// <param name="idVer">The id and version of the package.</param>
        /// <param name="target">The target context.</param>
        /// <param name="status">The result of the documentation request.</param>
        /// <param name="logUri">The URI of the detailed log of the documentation generation.</param>
        /// <param name="jsonUri">The URI of the JSON result. May be <c>null</c>.</param>
        Task SetRecordAsync(NugetPackageIdVersion idVer, PlatformTarget target, Status status, Uri logUri, Uri jsonUri);
    }

    public sealed class AzurePackageJsonTable : IPackageJsonTable
    {
        /// <summary>
        /// The version of the table schema
        /// </summary>
        private const int Version = 0;
        private readonly CloudTable _table;

        public static string TableName { get; } = "packagejson" + Version + "x" + JsonFactory.Version;

        public AzurePackageJsonTable(CloudTable table)
        {
            _table = table;
        }

        public async Task<(Status Status, Uri LogUri, Uri JsonUri)?> TryGetRecordAsync(NugetPackageIdVersion idVer, PlatformTarget target)
        {
            var entity = await Entity.FindOrDefaultAsync(_table, idVer, target).ConfigureAwait(false);
            if (entity == null)
                return null;
            return (entity.Status, entity.LogUri, entity.JsonUri);
        }

        public Task SetRecordAsync(NugetPackageIdVersion idVer, PlatformTarget target, Status status, Uri logUri, Uri jsonUri)
        {
            var entity = new Entity(_table, idVer, target)
            {
                Status = status,
                LogUri = logUri,
                JsonUri = jsonUri,
            };
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
