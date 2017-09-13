using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace DotNetApis.Common
{
    public sealed class AsyncLocalLogger : ILogger
    {
        private static readonly AsyncLocal<ILogger> ImplicitLogger = new AsyncLocal<ILogger>();

        private readonly ILogger _logger;

        public AsyncLocalLogger()
        {
            _logger = Logger;
        }

        public static ILogger Logger
        {
            get => ImplicitLogger.Value ?? NullLogger.Instance;
            set => ImplicitLogger.Value = value;
        }

        IDisposable ILogger.BeginScope<TState>(TState state) => _logger.BeginScope(state);
        bool ILogger.IsEnabled(LogLevel logLevel) => _logger.IsEnabled(logLevel);
        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) =>
            _logger.Log(logLevel, eventId, state, exception, formatter);
    }
}
