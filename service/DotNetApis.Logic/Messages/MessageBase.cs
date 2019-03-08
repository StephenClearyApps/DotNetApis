using System;
using System.Collections.Generic;
using DotNetApis.Common;

namespace DotNetApis.Logic.Messages
{
    /// <summary>
    /// The base type for all messages.
    /// </summary>
    public abstract class MessageBase
    {
        /// <summary>
        /// The AppInsights operation id.
        /// </summary>
        public Guid OperationId { get; set; } = AmbientContext.OperationId;

        /// <summary>
        /// The AppInsights parent operation id.
        /// </summary>
        public Guid ParentOperationId { get; set; } = AmbientContext.ParentOperationId;
    }
}
