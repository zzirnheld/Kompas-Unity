using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Helpers
{
	public static class EnumerableHelper
	{
		public static IEnumerable<(int index, T value)> Enumerate<T>(this IEnumerable<T> coll)
				=> coll.Select((i, val) => (val, i));
	}

	public static class ListHelper
	{
		public static List<T> AddRangeWithCast<T, U>(this List<T> list, IEnumerable<U> enumerable)
		{
			foreach(var obj in enumerable)
			{
				if (obj is T t) list.Add(t);
				else throw new System.ArgumentException($"Mismatch between type of object {obj.GetType()} and type parameter for adding range {typeof(T)}");
			}
			return list;
		}
	}
}