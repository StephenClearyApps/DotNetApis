using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace DotNetApis.Common
{
    public sealed class AsyncLocalLoggerFactory : ILoggerFactory
    {
        private static readonly AsyncLocal<ILoggerFactory> ImplicitLoggerFactory = new AsyncLocal<ILoggerFactory>();

        private readonly ILoggerFactory _loggerFactory;

        public AsyncLocalLoggerFactory()
        {
            _loggerFactory = LoggerFactory;
        }

        public static ILoggerFactory LoggerFactory
        {
            get => ImplicitLoggerFactory.Value;
            set => ImplicitLoggerFactory.Value = value;
        }

        // SimpleInjectory's Verify creates and then disposes its instances, so we do not dispose our (AsyncLocal) instance.
        void IDisposable.Dispose() { }
        ILogger ILoggerFactory.CreateLogger(string categoryName) => _loggerFactory?.CreateLogger(categoryName);
        void ILoggerFactory.AddProvider(ILoggerProvider provider) => _loggerFactory?.AddProvider(provider);
    }
}
