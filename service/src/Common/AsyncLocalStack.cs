using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public sealed class AsyncLocalStack<T>
    {
        private readonly AsyncLocal<ImmutableStack<T>> _asyncLocal = new AsyncLocal<ImmutableStack<T>>();

        private ImmutableStack<T> Current
        {
            get => _asyncLocal.Value ?? ImmutableStack<T>.Empty;
            set => _asyncLocal.Value = value;
        }

        public IImmutableStack<T> CurrentStack => _asyncLocal.Value ?? ImmutableStack<T>.Empty;

        public bool IsEmpty => Current.IsEmpty;

        public T Peek() => Current.Peek();

        public void Push(T value) => Current = Current.Push(value);

        public void Pop() => Current = Current.Pop();

        public void Pop(out T value) => Current = Current.Pop(out value);
    }
}
