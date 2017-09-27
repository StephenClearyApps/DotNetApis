using System;
using System.Collections.Generic;
using DotNetApis.Common.LogStructure;
using Microsoft.Extensions.Logging;

namespace DotNetApis.Common
{
    /// <summary>
    /// A logger that writes messages to a an in-memory list.
    /// </summary>
    public sealed class InMemoryLogger : ILogger
    {
        public List<LogMessage> Messages { get; } = new List<LogMessage>();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Messages.Add(new LogMessage
            {
                Type = logLevel,
                Timestamp = DateTimeOffset.UtcNow,
                Message = formatter(state, exception),
            });
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();
    }
}
