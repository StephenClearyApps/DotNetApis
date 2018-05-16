using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DotNetApis.Common
{
    public static class Logger
    {
        public static void Log<TLoggerCategory>(ILogger<TLoggerCategory> logger, int eventId, LogLevel logLevel, string format, Exception exception, [CallerMemberName] string eventName = null)
        {
            var action = (Action<ILogger, Exception>)DelegatesFor<TLoggerCategory>.Delegates.GetOrAdd((eventId, eventName, logLevel, format), _ =>
                LoggerMessage.Define(logLevel, new EventId(eventId, eventName), format));
            action(logger, exception);
        }

        public static void Log<TLoggerCategory, T1>(ILogger<TLoggerCategory> logger, int eventId, LogLevel logLevel, string format, T1 arg1, Exception exception, [CallerMemberName] string eventName = null)
        {
            var action = (Action<ILogger, T1, Exception>)DelegatesFor<TLoggerCategory>.Delegates.GetOrAdd((eventId, eventName, logLevel, format), _ =>
                LoggerMessage.Define<T1>(logLevel, new EventId(eventId, eventName), format));
            action(logger, arg1, exception);
        }

        public static void Log<TLoggerCategory, T1, T2>(ILogger<TLoggerCategory> logger, int eventId, LogLevel logLevel, string format, T1 arg1, T2 arg2, Exception exception, [CallerMemberName] string eventName = null)
        {
            var action = (Action<ILogger, T1, T2, Exception>)DelegatesFor<TLoggerCategory>.Delegates.GetOrAdd((eventId, eventName, logLevel, format), _ =>
                LoggerMessage.Define<T1, T2>(logLevel, new EventId(eventId, eventName), format));
            action(logger, arg1, arg2, exception);
        }

        public static void Log<TLoggerCategory, T1, T2, T3>(ILogger<TLoggerCategory> logger, int eventId, LogLevel logLevel, string format, T1 arg1, T2 arg2, T3 arg3, Exception exception, [CallerMemberName] string eventName = null)
        {
            var action = (Action<ILogger, T1, T2, T3, Exception>)DelegatesFor<TLoggerCategory>.Delegates.GetOrAdd((eventId, eventName, logLevel, format), _ =>
                LoggerMessage.Define<T1, T2, T3>(logLevel, new EventId(eventId, eventName), format));
            action(logger, arg1, arg2, arg3, exception);
        }

        public static void Log<TLoggerCategory, T1, T2, T3, T4>(ILogger<TLoggerCategory> logger, int eventId, LogLevel logLevel, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception exception, [CallerMemberName] string eventName = null)
        {
            var action = (Action<ILogger, T1, T2, T3, T4, Exception>)DelegatesFor<TLoggerCategory>.Delegates.GetOrAdd((eventId, eventName, logLevel, format), _ =>
                LoggerMessage.Define<T1, T2, T3, T4>(logLevel, new EventId(eventId, eventName), format));
            action(logger, arg1, arg2, arg3, arg4, exception);
        }

        public static void Log<TLoggerCategory, T1, T2, T3, T4, T5>(ILogger<TLoggerCategory> logger, int eventId, LogLevel logLevel, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception exception, [CallerMemberName] string eventName = null)
        {
            var action = (Action<ILogger, T1, T2, T3, T4, T5, Exception>)DelegatesFor<TLoggerCategory>.Delegates.GetOrAdd((eventId, eventName, logLevel, format), _ =>
                LoggerMessage.Define<T1, T2, T3, T4, T5>(logLevel, new EventId(eventId, eventName), format));
            action(logger, arg1, arg2, arg3, arg4, arg5, exception);
        }

        public static void Log<TLoggerCategory, T1, T2, T3, T4, T5, T6>(ILogger<TLoggerCategory> logger, int eventId, LogLevel logLevel, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Exception exception, [CallerMemberName] string eventName = null)
        {
            var action = (Action<ILogger, T1, T2, T3, T4, T5, T6, Exception>)DelegatesFor<TLoggerCategory>.Delegates.GetOrAdd((eventId, eventName, logLevel, format), _ =>
                LoggerMessage.Define<T1, T2, T3, T4, T5, T6>(logLevel, new EventId(eventId, eventName), format));
            action(logger, arg1, arg2, arg3, arg4, arg5, arg6, exception);
        }

        private static class DelegatesFor<TLoggerCategory>
        {
            public static readonly ConcurrentDictionary<(int eventId, string eventName, LogLevel logLevel, string format), object> Delegates = new ConcurrentDictionary<(int, string, LogLevel, string), object>();
        }
    }
}
