using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ServerEffectStack
{
    private readonly List<IServerStackable> stack;

    public bool Empty => stack.Count == 0;

    public ServerEffectStack()
    {
        stack = new List<IServerStackable>();
    }

    public void Push(IServerStackable entry)
    {
        stack.Add(entry);
    }

    public IServerStackable Pop()
    {
        if (stack.Count == 0) return null;

        var last = stack.Last();
        stack.Remove(last);
        return last;
    }

    public IServerStackable Cancel(int index)
    {
        if (index >= stack.Count) return null;

        var canceled = stack[index];
        stack.RemoveAt(index);
        return canceled;
    }
}
