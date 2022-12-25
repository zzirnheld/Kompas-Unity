using System.Linq;
using System.Collections.Generic;

namespace KompasCore.Effects.Selectors
{
    public interface ISelector<T>
    {
        T Select(IReadOnlyCollection<T> objects);
    }

    public class RandomSelector<T> : ISelector<T>
    {
        System.Random random = new System.Random();

        public T Select(IReadOnlyCollection<T> objects) => objects.ElementAt(random.Next(0, objects.Count));
    }
}