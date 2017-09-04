﻿using System;
using System.Collections.Generic;
using DotNetApis.Common;

namespace DotNetApis.Logic.Messages
{
    /// <summary>
    /// The base type for all messages.
    /// </summary>
    public abstract class MessageBase
    {
        protected MessageBase(bool includeLog)
        {
            if (includeLog)
                Log = AmbientContext.InMemoryLogger?.Messages;
        }

        /// <summary>
        /// The AppInsights operation id.
        /// </summary>
        public Guid OperationId { get; set; } = AmbientContext.OperationId;

        /// <summary>
        /// The AppInsights parent operation id.
        /// </summary>
        public Guid ParentOperationId { get; set; } = AmbientContext.ParentOperationId;

        /// <summary>
        /// The HTTP request id.
        /// </summary>
        public string RequestId { get; set; } = AmbientContext.RequestId;

        /// <summary>
        /// The trace log for this request.
        /// </summary>
        public IReadOnlyList<string> Log { get; }
    }
}