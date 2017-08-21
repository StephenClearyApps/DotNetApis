using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Common.Internals;
using Microsoft.Azure.WebJobs.Host;

namespace Common
{
    public interface ILogger
    {
        void Trace(string message);
    }

    /// <summary>
    /// A logger that writes messages to all loggers defined in the ambient context.
    /// </summary>
    public sealed class AmbientCompositeLogger : ILogger
    {
        private readonly IEnumerable<ILogger> _loggers;

        public AmbientCompositeLogger()
        {
            _loggers = AmbientContext.Loggers;
        }

        void ILogger.Trace(string message) => _loggers.Do(x => x.Trace(message));
    }
}
