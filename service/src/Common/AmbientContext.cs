using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// Objects that have a lifetime outside of the DI container. Properties retrieve the current value for the current scope.
    /// </summary>
    public static class AmbientContext
    {
        private static readonly AsyncLocal<IImmutableSet<ILogger>> _loggers = new AsyncLocal<IImmutableSet<ILogger>>();

        public static IEnumerable<ILogger> Loggers => _loggers.Value;

        /// <summary>
        /// Sets the values for the current scope (and child scopes).
        /// </summary>
        public static void Initialize(IEnumerable<ILogger> loggers)
        {
            _loggers.Value = loggers.ToImmutableHashSet();
        }
    }
}
