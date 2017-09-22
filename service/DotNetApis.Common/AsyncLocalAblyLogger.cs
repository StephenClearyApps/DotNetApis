using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DotNetApis.Common.Internals;
using Microsoft.Extensions.Logging;

namespace DotNetApis.Common
{
    /// <summary>
    /// A logger that attempts to write to an implicit Ably channel. This type can be safely created before its channel is created.
    /// </summary>
    public sealed class AsyncLocalAblyLogger : ILogger
    {
        private static readonly AsyncLocal<AblyChannel> ImplicitChannel = new AsyncLocal<AblyChannel>();

        public static void TryCreate(string channelName, ILogger logger)
        {
            try
            {
                ImplicitChannel.Value = AblyService.CreateLogChannel(channelName);
            }
            catch (Exception ex)
            {
                logger.LogWarning(0, ex, "Could not initialize Ably: {exceptionMessage}", ex.Message);
            }
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            ImplicitChannel.Value?.LogMessage(logLevel.ToString(), formatter(state, exception));
        }

        public bool IsEnabled(LogLevel logLevel) => ImplicitChannel.Value != null && logLevel > LogLevel.Information;

        public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();
    }
}
