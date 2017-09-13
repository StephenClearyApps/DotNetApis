using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DotNetApis.Common
{
    /// <summary>
    /// Objects that have a lifetime outside of the DI container. Properties retrieve the current value for the current scope.
    /// </summary>
    public static class AmbientContext
    {
        private static readonly AsyncLocal<Guid> _operationId = new AsyncLocal<Guid>();
        private static readonly AsyncLocal<Guid> _parentOperationId = new AsyncLocal<Guid>();
        private static readonly AsyncLocal<string> _requestId = new AsyncLocal<string>();

        public static Guid OperationId => _operationId.Value;
        public static Guid ParentOperationId
        {
            get => _parentOperationId.Value;
            set => _parentOperationId.Value = value;
        }
        public static string RequestId => _requestId.Value;

        /// <summary>
        /// Sets the values for HTTP-triggered API functions.
        /// </summary>
        public static void InitializeForHttpApi(string requestId, Guid operationId)
        {
            _operationId.Value = operationId;
            _requestId.Value = requestId;
        }

        /// <summary>
        /// Sets the values for backend queue-triggered functions.
        /// </summary>
        //public static void InitializeForQueueProcessing(ILogger log, TraceWriter writer, Guid operationId)
        //{
        //    var inMemoryLogger = new InMemoryLogger();
        //    _loggers.Value = Enumerables.Return(inMemoryLogger, log, new TraceWriterLogger(writer)).ToImmutableHashSet(); // TODO: Ably
        //    _inMemoryLogger.Value = inMemoryLogger;
        //    _operationId.Value = operationId;
        //}

        /// <summary>
        /// Sets the values for manual HTTP-triggered functions.
        /// </summary>
        //public static void InitializeForManualHttpTrigger(ILogger log, TraceWriter writer, bool requestIsLocal, string requestId, Guid operationId)
        //{
        //    _loggers.Value = Enumerables.Return(log, requestIsLocal ? new TraceWriterLogger(writer) : null).ToImmutableHashSet();
        //    _operationId.Value = operationId;
        //    _requestId.Value = requestId;
        //}
    }
}
