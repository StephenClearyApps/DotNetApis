using System;
using System.Collections.Generic;
using DotNetApis.Common;
using DotNetApis.Common.LogStructure;

namespace DotNetApis.Logic.Messages
{
    /// <summary>
    /// The base type for messages that include a trace log.
    /// </summary>
    public abstract class MessageBaseWithLog : MessageBase
    {
        /// <summary>
        /// The trace log for this request.
        /// </summary>
        public IReadOnlyList<LogMessage> Log { get; } = AmbientContext.InMemoryLoggerProvider?.Messages;
    }
}
