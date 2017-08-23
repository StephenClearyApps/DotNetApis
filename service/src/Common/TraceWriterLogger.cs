using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Common
{
    /// <summary>
    /// A logger that writes to a <see cref="TraceWriter"/>.
    /// </summary>
    public sealed class TraceWriterLogger : ILogger
    {
        private readonly TraceWriter _writer;

        public TraceWriterLogger(TraceWriter writer)
        {
            _writer = writer;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) => 
            _writer.Trace(new TraceEvent(Map(logLevel), formatter(state, exception), null, exception));

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        private static TraceLevel Map(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return TraceLevel.Verbose;
                case LogLevel.Information:
                case LogLevel.Debug:
                    return TraceLevel.Info;
                case LogLevel.Warning:
                    return TraceLevel.Warning;
                default:
                    return TraceLevel.Error;
            }
        }
    }
}
