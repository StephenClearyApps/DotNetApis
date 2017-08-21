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
    /// A logger that writes messages to a <see cref="TextWriter"/> as well as keeping an in-memory copy.
    /// </summary>
    public sealed class Logger : ILogger
    {
        private readonly TraceWriter _writer;

        public Logger()
        {
            _writer = AsyncContext.TraceWriter;
        }

        public List<string> Messages { get; } = new List<string>();

        public void Trace(string message)
        {
            Messages.Add(message);
            _writer?.Info(message);
        }
    }
}
