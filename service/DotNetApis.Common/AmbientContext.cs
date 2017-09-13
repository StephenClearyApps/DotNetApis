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
        private static readonly AsyncLocal<Guid> ImplicitOperationId = new AsyncLocal<Guid>();
        private static readonly AsyncLocal<Guid> ImplicitParentOperationId = new AsyncLocal<Guid>();
        private static readonly AsyncLocal<string> ImplicitRequestId = new AsyncLocal<string>();
        private static readonly AsyncLocal<InMemoryLogger> ImplicitInMemoryLogger = new AsyncLocal<InMemoryLogger>();

        public static Guid OperationId
        {
            get => ImplicitOperationId.Value;
            set => ImplicitOperationId.Value = value;
        }
        public static Guid ParentOperationId
        {
            get => ImplicitParentOperationId.Value;
            set => ImplicitParentOperationId.Value = value;
        }

        /// <summary>
        /// May return <c>null</c>.
        /// </summary>
        public static string RequestId
        {
            get => ImplicitRequestId.Value;
            set => ImplicitRequestId.Value = value;
        }

        /// <summary>
        /// May return <c>null</c>.
        /// </summary>
        public static InMemoryLogger InMemoryLogger
        {
            get => ImplicitInMemoryLogger.Value;
            set => ImplicitInMemoryLogger.Value = value;
        }

        /// <summary>
        /// Sets the values for HTTP-triggered API functions.
        /// </summary>
        public static void InitializeForHttpApi(string requestId, Guid operationId)
        {
            ImplicitOperationId.Value = operationId;
            ImplicitRequestId.Value = requestId;
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
