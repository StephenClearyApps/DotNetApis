using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.Disposables;

namespace DotNetApis.Common
{
    /// <summary>
    /// A base type for types whose values are implicitly-scoped.
    /// </summary>
    /// <typeparam name="T">The derived type.</typeparam>
    public abstract class ScopeBase<T> where T : ScopeBase<T>
    {
        private static readonly AsyncLocalStack<T> Stack = new AsyncLocalStack<T>();

        /// <summary>
        /// Creates a new scope with the specified value as the current value. When the returned disposable is disposed, the scope is destroyed and the previous value is restored.
        /// </summary>
        /// <param name="value">The value for the new scope.</param>
        protected static IDisposable Create(T value)
        {
            Stack.Push(value);
            return new AnonymousDisposable(() => Stack.Pop());
        }

        /// <summary>
        /// Provides implicit access to values saved in a scope.
        /// </summary>
        public sealed class Accessor
        {
            /// <summary>
            /// Gets the value for the current scope.
            /// </summary>
            public T Current => Stack.TryPeek();
        }
    }
}
