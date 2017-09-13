using System;
using System.Collections.Generic;
using DotNetApis.Common;

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
        public IReadOnlyList<string> Log { get; } = AmbientContext.InMemoryLogger?.Messages;
    }
}
