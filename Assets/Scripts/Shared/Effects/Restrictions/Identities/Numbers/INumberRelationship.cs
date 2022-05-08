using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    public interface INumberRelationship
    {
        public int Apply(ICollection<int> numbers);
    }

    public class MaximumNumberRelationship : INumberRelationship
    {
        public int Apply(ICollection<int> numbers) => numbers.Max();
    }

    public class MinimumNumberRelationship : INumberRelationship
    {
        public int Apply(ICollection<int> numbers) => numbers.Min();
    }
}