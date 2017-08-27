using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Common
{
    /// <summary>
    /// Objects that have a lifetime outside of the DI container. Properties retrieve the current value for the current scope.
    /// </summary>
    public static class AmbientContext
    {
        private static readonly AsyncLocal<InMemoryLogger> _inMemoryLogger = new AsyncLocal<InMemoryLogger>();
        private static readonly AsyncLocal<IImmutableSet<ILogger>> _loggers = new AsyncLocal<IImmutableSet<ILogger>>();
        private static readonly AsyncLocal<Guid> _operationId = new AsyncLocal<Guid>();
        private static readonly AsyncLocal<Guid> _parentOperationId = new AsyncLocal<Guid>();
        private static readonly AsyncLocal<string> _requestId = new AsyncLocal<string>();

        public static IEnumerable<ILogger> Loggers => _loggers.Value;
        public static InMemoryLogger InMemoryLogger => _inMemoryLogger.Value;
        public static Guid OperationId => _operationId.Value;
        public static Guid ParentOperationId
        {
            get => _parentOperationId.Value;
            set => _parentOperationId.Value = value;
        }
        public static string RequestId => _requestId.Value;

        /// <summary>
        /// Sets the values for the Documentation HTTP-triggered function.
        /// </summary>
        public static void Initialize(ILogger log, TraceWriter writer, bool requestIsLocal, string requestId, Guid operationId)
        {
            var inMemoryLogger = new InMemoryLogger();
            _loggers.Value = Enumerables.Return(inMemoryLogger, log, requestIsLocal ? new TraceWriterLogger(writer) : null).ToImmutableHashSet();
            _inMemoryLogger.Value = inMemoryLogger;
            _operationId.Value = operationId;
            _requestId.Value = requestId;
        }

        /// <summary>
        /// Sets the values for the Generate queue-triggered function.
        /// </summary>
        public static void Initialize(ILogger log, TraceWriter writer, Guid operationId)
        {
            var inMemoryLogger = new InMemoryLogger();
            _loggers.Value = Enumerables.Return(inMemoryLogger, log, new TraceWriterLogger(writer)).ToImmutableHashSet(); // TODO: Ably
            _inMemoryLogger.Value = inMemoryLogger;
            _operationId.Value = operationId;
        }
    }
}
