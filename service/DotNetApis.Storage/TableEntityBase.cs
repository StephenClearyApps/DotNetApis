using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace DotNetApis.Storage
{
    /// <summary>
    /// Helper type for entities stored in Azure tables.
    /// </summary>
    public abstract class TableEntityBase
    {
        private readonly CloudTable _table;

        /// <summary>
        /// Creates a new entity that is not yet in the table.
        /// </summary>
        /// <param name="table">The Azure table.</param>
        /// <param name="partitionKey">The partition key for this entity.</param>
        /// <param name="rowKey">The row key for this entity.</param>
        protected TableEntityBase(CloudTable table, string partitionKey, string rowKey)
        {
            _table = table;
            Entity = new DynamicTableEntity();
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        /// <summary>
        /// Creates an entity object for an entity already in the table.
        /// </summary>
        /// <param name="table">The Azure table.</param>
        /// <param name="entity">The entity read from the table.</param>
        protected TableEntityBase(CloudTable table, DynamicTableEntity entity)
        {
            _table = table;
            Entity = entity;
        }

        protected DynamicTableEntity Entity { get; }

        protected string PartitionKey
        {
            get => Entity.PartitionKey;
            set => Entity.PartitionKey = value;
        }

        protected string RowKey
        {
            get => Entity.RowKey;
            set => Entity.RowKey = value;
        }

        /// <summary>
        /// Gets a string property from the entity. Returns <paramref name="defaultValue"/> if the entity does not have that property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="defaultValue">The default value to return if the property is not found.</param>
        protected string Get(string propertyName, string defaultValue) => Entity.Properties.TryGetValue(propertyName, out var result) ? result.StringValue ?? defaultValue : defaultValue;

        /// <summary>
        /// Sets a string property for the entity.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="value">The value to store in the property.</param>
        protected void Set(string propertyName, string value) => Entity.Properties[propertyName] = EntityProperty.GeneratePropertyForString(value);

        /// <summary>
        /// Gets an int property from the entity. Returns <paramref name="defaultValue"/> if the entity does not have that property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="defaultValue">The default value to return if the property is not found.</param>
        protected int Get(string propertyName, int defaultValue) => Entity.Properties.TryGetValue(propertyName, out var result) ? result.Int32Value ?? defaultValue : defaultValue;

        /// <summary>
        /// Sets an int property for the entity.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="value">The value to store in the property.</param>
        protected void Set(string propertyName, int value) => Entity.Properties[propertyName] = EntityProperty.GeneratePropertyForInt(value);

        /// <summary>
        /// Adds this entity to the table if it is not already there; otherwise, updates the table entity with this entity.
        /// </summary>
        public Task InsertOrReplaceAsync() => _table.ExecuteAsync(TableOperation.InsertOrReplace(Entity));
    }
}
