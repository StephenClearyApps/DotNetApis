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
    public sealed class AsyncLocalAblyLoggerProvider : ILoggerProvider
    {
        private static readonly AsyncLocal<AblyChannel> ImplicitChannel = new AsyncLocal<AblyChannel>();

        public static void TryCreate(string channelName, ILoggerFactory loggerFactory)
        {
            try
            {
                ImplicitChannel.Value = AblyService.CreateLogChannel(channelName);
            }
            catch (Exception ex)
            {
                loggerFactory.CreateLogger<Logging.AblyLogger>().AblyInitializationFailed(ex);
            }
        }

        void IDisposable.Dispose() { }

        public ILogger CreateLogger(string categoryName) => new AsyncLocalAblyLogger();

        private sealed class AsyncLocalAblyLogger : ILogger
        {
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                if (IsEnabled(logLevel))
                {
                    var message = formatter(state, exception) ?? "";
                    if (exception != null)
                    {
                        if (message != "")
                            message += ": ";
                        message += $"[{exception.GetType().Name}]: {exception.Message}";
                    }

                    ImplicitChannel.Value?.LogMessage(logLevel.ToString(), message);
                }
            }

            public bool IsEnabled(LogLevel logLevel) => ImplicitChannel.Value != null && logLevel >= LogLevel.Information && logLevel != LogLevel.Warning;

            public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();
        }
    }

    internal static partial class Logging
    {
        public static void AblyInitializationFailed(this ILogger<AblyLogger> logger, Exception ex) =>
            Logger.Log(logger, 1, LogLevel.Warning, "Could not initialize Ably", ex);

        public sealed class AblyLogger { }
    }
}
