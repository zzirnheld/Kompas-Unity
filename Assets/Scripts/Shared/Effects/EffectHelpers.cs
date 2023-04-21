using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects
{
	internal static class EffectHelpers
	{
		public static T GetItem<T>(ICollection<T> collection, int index)
			=> collection.ElementAtOrDefault(TrueIndex(collection.Count, index));

		public static int TrueIndex(int len, int index) => index < 0 ? index + len : index;
	}
}