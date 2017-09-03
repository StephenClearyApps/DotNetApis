using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetApis.Common
{
    public static class CollectionExtensions
    {
        public static IReadOnlyCollection<T> Reify<T>(this IEnumerable<T> source)
        {
            if (source is IReadOnlyCollection<T> result)
                return result;
            if (source is ICollection<T> collection)
                return new CollectionWrapper<T>(collection);

            return new List<T>(source);
        }

        private sealed class CollectionWrapper<T> : IReadOnlyCollection<T>
        {
            private readonly ICollection<T> _collection;

            public CollectionWrapper(ICollection<T> collection)
            {
                _collection = collection;
            }

            public int Count => _collection.Count;

            public IEnumerator<T> GetEnumerator() => _collection.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => _collection.GetEnumerator();
        }
    }
}
