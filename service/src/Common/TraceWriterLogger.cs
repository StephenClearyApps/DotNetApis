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
    /// <summary>
    /// A logger that writes messages to a <see cref="TraceWriter"/>.
    /// </summary>
    public sealed class TraceWriterLogger : ILogger
    {
        private readonly TraceWriter _writer;

        public TraceWriterLogger(TraceWriter writer)
        {
            _writer = writer;
        }

        void ILogger.Trace(string message) => _writer?.Info(message);
    }
}
