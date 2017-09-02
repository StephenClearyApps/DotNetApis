using System;
using System.Collections.Generic;

namespace DotNetApis.Common
{
    public static class Enumerables
    {
        public static void Do<T>(this IEnumerable<T> @this, Action<T> action)
        {
            foreach (var item in @this)
                action(item);
        }

        public static IEnumerable<T> Return<T>(params T[] items) => items;
    }
}
