using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Helpers
{
    public static class EnumerableHelper
    {
        public static IEnumerable<(int index, T value)> Enumerate<T>(this IEnumerable<T> coll)
                => coll.Select((i, val) => (val, i));
    }
}