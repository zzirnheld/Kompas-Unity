using KompasCore.Effects;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects
{
    public class ServerEffectStack
    {
        private readonly List<(IServerStackable stackable, ActivationContext context)> stack
            = new List<(IServerStackable stackable, ActivationContext)>();

        public bool Empty => stack.Count == 0;
        public int Count => stack.Count;

        public void Push((IServerStackable, ActivationContext) entry)
        {
            stack.Add(entry);
        }

        public (IServerStackable, ActivationContext) Pop()
        {
            if (stack.Count == 0) return (default, default);

            var last = stack.Last();
            stack.Remove(last);
            return last;
        }

        public IServerStackable Cancel(int index)
        {
            if (index >= stack.Count) return null;

            var canceled = stack[index].stackable;
            stack.RemoveAt(index);
            return canceled;
        }
    }
}