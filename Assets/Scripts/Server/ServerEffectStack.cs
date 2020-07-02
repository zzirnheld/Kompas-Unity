using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ServerEffectStack
{
    private readonly List<(IServerStackable stackable, int startIndex)> stack 
        = new List<(IServerStackable stackable, int startIndex)>();

    public bool Empty => stack.Count == 0;

    public void Push((IServerStackable, int) entry)
    {
        stack.Add(entry);
    }

    public (IServerStackable, int) Pop()
    {
        if (stack.Count == 0) return (null, 0);

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
