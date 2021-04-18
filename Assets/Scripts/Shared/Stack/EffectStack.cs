using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects
{
    public class EffectStack<StackableType>
    {
        private readonly List<(StackableType stackable, ActivationContext context)> stack
            = new List<(StackableType stackable, ActivationContext)>();

        public IEnumerable<StackableType> StackEntries => stack.Select(entry => entry.stackable);

        public bool Empty => stack.Count == 0;
        public int Count => stack.Count;

        public void Push((StackableType, ActivationContext) entry)
        {
            stack.Add(entry);
        }

        public (StackableType, ActivationContext) Pop()
        {
            if (stack.Count == 0) return (default, default);

            var last = stack.Last();
            stack.Remove(last);
            return last;
        }

        public StackableType Cancel(int index)
        {
            if (index >= stack.Count) return default(StackableType);

            var canceled = stack[index].stackable;
            stack.RemoveAt(index);
            return canceled;
        }
    }
}