using System.Collections.Generic;
using Common;

namespace FunctionApp.Messages
{
    /// <summary>
    /// The base type for messages returned as HTTP responses from the HTTP trigger function.
    /// </summary>
    public abstract class ResponseMessageBase : MessageBase
    {
        /// <summary>
        /// The trace log for this request.
        /// </summary>
        public IReadOnlyList<string> Log { get; } = AmbientContext.InMemoryLogger?.Messages;
    }
}
