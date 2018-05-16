using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace DotNetApis.Common
{
    public static class TraceDump
    {
        /// <summary>
        /// Converts all elements in the sequence to strings and dumps them.
        /// </summary>
        public static string Dump<T>(this IEnumerable<T> source) => JsonConvert.SerializeObject(source.Select(x => x.ToString()), Formatting.None);

        /// <summary>
        /// Converts all keys and values in the dictionary to strings and dumps them.
        /// </summary>
        public static string Dump<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source) =>
            JsonConvert.SerializeObject(new SortedList<string, string>(source.ToDictionary(x => x.Key.ToString(), x => x.Value.ToString())), Formatting.None);

        /// <summary>
        /// Creates a wrapper around this list that supports <c>ToString</c> by calling <see cref="Dump{T}(IEnumerable{T})"/>.
        /// </summary>
        /// <typeparam name="T">The type of the list elements.</typeparam>
        /// <param name="source">The source list.</param>
        public static IReadOnlyList<T> Dumpable<T>(this IReadOnlyList<T> source) => new ListWrapper<T>(source);
        
        /// <summary>
        /// Creates a wrapper around this enumerable that supports <c>ToString</c> by calling <see cref="Dump{T}(IEnumerable{T})"/>.
        /// </summary>
        /// <typeparam name="T">The type of the list elements.</typeparam>
        /// <param name="source">The source list.</param>
        public static IEnumerable<T> Dumpable<T>(this IEnumerable<T> source) => new EnumerableWrapper<T>(source);

        private sealed class ListWrapper<T>: IReadOnlyList<T>
        {
            private readonly IReadOnlyList<T> _source;

            public ListWrapper(IReadOnlyList<T> source)
            {
                _source = source;
            }

            public IEnumerator<T> GetEnumerator() => _source.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_source).GetEnumerator();

            public int Count => _source.Count;

            public T this[int index] => _source[index];

            public override string ToString() => this.Dump();
        }

        private sealed class EnumerableWrapper<T>: IEnumerable<T>
        {
            private readonly IEnumerable<T> _source;

            public EnumerableWrapper(IEnumerable<T> source)
            {
                _source = source;
            }

            public IEnumerator<T> GetEnumerator() => _source.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_source).GetEnumerator();

            public override string ToString() => this.Dump();
        }
    }
}
