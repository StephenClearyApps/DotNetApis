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
    }
}
