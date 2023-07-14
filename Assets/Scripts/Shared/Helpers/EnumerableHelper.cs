using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Helpers
{
	public static class EnumerableHelper
	{
		public static IEnumerable<(int index, T value)> Enumerate<T>(this IEnumerable<T> coll)
				=> coll.Select((i, val) => (val, i));

		public static IEnumerable<T> Safe<T>(this IEnumerable<T> source)
		{
			if (source == null) yield break;

			foreach (var item in source) yield return item;
		}

		public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T elem)
			=> source.Concat(new[] { elem });
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