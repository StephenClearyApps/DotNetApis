using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
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
