using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace DotNetApis.Storage
{
    /// <summary>
    /// Represents an action to be taken as part of a batch.
    /// </summary>
    public interface IBatchAction
    {
    }

    /// <summary>
    /// Represents a collection of actions to execute as a batch.
    /// </summary>
    public interface IBatch
    {
        /// <summary>
        /// The number of actions in this batch.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Adds an action to this batch. Throws an exception if the type of the batch action is not compatible with the type of the batch.
        /// </summary>
        /// <param name="action">The action to add.</param>
        void Add(IBatchAction action);

        /// <summary>
        /// Executes all the actions in the batch.
        /// </summary>
        Task ExecuteAsync();
    }

    public sealed class AzureTableBatch : IBatch
    {
        private readonly CloudTable _table;
        private readonly TableBatchOperation _operation;

        public AzureTableBatch(CloudTable table)
        {
            _table = table;
            _operation = new TableBatchOperation();
        }

        public int Count => _operation.Count;

        public void Add(IBatchAction action)
        {
            if (action is InsertOrReplaceAction insertOrReplaceAction)
            {
                insertOrReplaceAction.Apply(this);
                return;
            }
            throw new InvalidOperationException($"Unknown batch action {action.GetType().Name}");
        }

        public Task ExecuteAsync() => _table.ExecuteBatchAsync(_operation);

        public sealed class InsertOrReplaceAction : IBatchAction
        {
            private readonly ITableEntity _entity;

            public InsertOrReplaceAction(ITableEntity entity)
            {
                _entity = entity;
            }

            public void Apply(AzureTableBatch batch) => batch._operation.InsertOrReplace(_entity);
        }
    }
}
