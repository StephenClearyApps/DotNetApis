using System.Collections.Immutable;
using System.Threading;

namespace DotNetApis.Common
{
    /// <summary>
    /// A stack that exists in an async-compatible implicit scope.
    /// </summary>
    /// <typeparam name="T">The type of objects contained in the stack.</typeparam>
    public sealed class AsyncLocalStack<T>
    {
        private readonly AsyncLocal<ImmutableStack<T>> _stack = new AsyncLocal<ImmutableStack<T>>();

        private ImmutableStack<T> Value
        {
            get => _stack.Value ?? ImmutableStack<T>.Empty;
            set => _stack.Value = value;
        }

        /// <summary>
        /// Pushes a value onto the implicit stack.
        /// </summary>
        /// <param name="value">The value to push.</param>
        public void Push(T value) => Value = Value.Push(value);

        /// <summary>
        /// Pops a value off the implicit stack.
        /// </summary>
        public void Pop() => Value = Value.Pop();

        /// <summary>
        /// Attempts to retrieve the topmost value from the implicit stack. Returns <c>default(T)</c> if the stack is empty.
        /// </summary>
        public T TryPeek() => Value.IsEmpty ? default(T) : Value.Peek();
    }
}