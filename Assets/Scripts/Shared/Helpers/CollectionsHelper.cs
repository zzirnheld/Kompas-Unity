using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Helpers
{
    public class CollectionsHelper
    {
        private static readonly System.Random rng = new System.Random();

        public static IReadOnlyCollection<T> Shuffle<T>(IReadOnlyCollection<T> list)
        {
            int n = list.Count;
            var shuffled = list.ToArray();
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (shuffled[n], shuffled[k]) = (shuffled[k], shuffled[n]);
            }
            return list;
        }
    }
}