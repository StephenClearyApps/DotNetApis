using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Storage
{
    public static class CloudTableExtensions
    {
        /// <summary>
        /// Performs a point search in an azure table. Returns <c>null</c> if the entity is not found.
        /// </summary>
        /// <param name="this">The azure table.</param>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="rowKey">The row key.</param>
        public static Task<DynamicTableEntity> FindOrDefaultAsync(this CloudTable @this, string partitionKey, string rowKey) => FindOrDefaultCore(@this, partitionKey, rowKey, sync: false);

        /// <summary>
        /// Performs a point search in an azure table. Returns <c>null</c> if the entity is not found.
        /// </summary>
        /// <param name="this">The azure table.</param>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="rowKey">The row key.</param>
        public static DynamicTableEntity FindOrDefault(CloudTable @this, string partitionKey, string rowKey) => FindOrDefaultCore(@this, partitionKey, rowKey, sync: true).GetAwaiter().GetResult();

        private static async Task<DynamicTableEntity> FindOrDefaultCore(CloudTable @this, string partitionKey, string rowKey, bool sync)
        {
            var operation = TableOperation.Retrieve(partitionKey, rowKey);
            var result = sync ? @this.Execute(operation) : await @this.ExecuteAsync(operation).ConfigureAwait(false);
            if ((HttpStatusCode)result.HttpStatusCode == HttpStatusCode.NotFound)
                return null;
            return (DynamicTableEntity)result.Result;
        }
    }
}
