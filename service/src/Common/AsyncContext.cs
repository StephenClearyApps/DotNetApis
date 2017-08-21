using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;

namespace Common
{
    /// <summary>
    /// Objects that have a lifetime outside of the DI container. Properties retrieve the current value for the current scope.
    /// </summary>
    public static class AsyncContext
    {
        private static readonly AsyncLocal<TraceWriter> _traceWriter = new AsyncLocal<TraceWriter>();

        public static TraceWriter TraceWriter => _traceWriter.Value;

        /// <summary>
        /// Sets the values for the current scope (and child scopes).
        /// </summary>
        public static void Initialize(TraceWriter traceWriter)
        {
            _traceWriter.Value = traceWriter;
        }
    }
}
