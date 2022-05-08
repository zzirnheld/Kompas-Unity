using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    public interface INumberSelector
    {
        public int Apply(ICollection<int> numbers);
    }

    public class MaximumNumberSelector : INumberSelector
    {
        public int Apply(ICollection<int> numbers) => numbers.Max();
    }

    public class MinimumNumberSelector : INumberSelector
    {
        public int Apply(ICollection<int> numbers) => numbers.Min();
    }
}