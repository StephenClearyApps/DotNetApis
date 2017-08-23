using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using IMicrosoftLogger = Microsoft.Extensions.Logging.ILogger;

namespace Common
{
    /// <summary>
    /// A logger that writes messages to a <see cref="IMicrosoftLogger"/>.
    /// </summary>
    public sealed class AzureFunctionsLogger : ILogger
    {
        private readonly IMicrosoftLogger _logger;

        public AzureFunctionsLogger(IMicrosoftLogger logger)
        {
            _logger = logger;
        }

        void ILogger.Trace(string message)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
                _logger.LogTrace(message);
        }
    }
}
