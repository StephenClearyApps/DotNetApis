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
    /// A logger that writes messages to a an in-memory list.
    /// </summary>
    public sealed class InMemoryLogger : ILogger
    {
        public List<string> Messages { get; } = new List<string>();

        void ILogger.Trace(string message) => Messages.Add(message);
    }
}
