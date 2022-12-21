using System.Linq;
using System.Collections.Generic;

namespace KompasCore.Effects.Selectors
{
    public interface Selector
    {
        T Select<T>(IReadOnlyCollection<T> objects);
    }

    public class RandomSelector : Selector
    {
        System.Random random = new System.Random();

        public T Select<T>(IReadOnlyCollection<T> objects) => objects.ElementAt(random.Next(0, objects.Count));
    }
}