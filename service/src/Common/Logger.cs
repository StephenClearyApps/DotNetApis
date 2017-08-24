using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Common.Internals;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Nito.Disposables;

namespace Common
{
    /// <summary>
    /// A logger that writes messages to all loggers defined in the ambient context.
    /// </summary>
    public sealed class AmbientCompositeLogger : ILogger
    {
        private readonly IEnumerable<ILogger> _loggers;

        public AmbientCompositeLogger()
        {
            _loggers = AmbientContext.Loggers.Where(x => x != null).ToList();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            => _loggers.Do(x => x.Log(logLevel, eventId, state, exception, formatter));

        public bool IsEnabled(LogLevel logLevel) => _loggers.Any(x => x.IsEnabled(logLevel));

        public IDisposable BeginScope<TState>(TState state) => CollectionDisposable.Create(_loggers.Select(x => x.BeginScope(state)));
    }
}
