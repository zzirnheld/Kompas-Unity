using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    public interface INumberSelector
    {
        public int Apply(ICollection<int> numbers);
    }

    namespace NumberSelectors
    {
        public class Maximum : INumberSelector
        {
            public int Apply(ICollection<int> numbers) => numbers.Max();
        }

        public class Minimum : INumberSelector
        {
            public int Apply(ICollection<int> numbers) => numbers.Min();
        }
    }
}