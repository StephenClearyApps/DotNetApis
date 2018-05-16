using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DotNetApis.Common
{
    /// <summary>
    /// A logger provider whose loggers all forward to a single underlying <see cref="ILogger"/>.
    /// </summary>
    public sealed class ForwardingLoggerProvider: ILoggerProvider
    {
        private readonly ILogger _logger;

        public ForwardingLoggerProvider(ILogger logger)
        {
            _logger = logger;
        }

        void IDisposable.Dispose() { }

        public ILogger CreateLogger(string categoryName) => _logger;
    }
}
