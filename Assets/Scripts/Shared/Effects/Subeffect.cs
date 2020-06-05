using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Subeffect
{
    public abstract Effect Effect { get; }
    public abstract Player Controller { get; }
    public abstract Game Game { get; }

    public int SubeffIndex { get; protected set; }

    public Card Source => Effect.Source;

    /// <summary>
    /// The index in the Effect.targets array for which target this effect uses.
    /// If positive, just an index.
    /// If negative, it's Effect.targets.Count + targetIndex (aka that many back from the end)
    /// </summary>
    public int TargetIndex = -1;

    public Card GetTarget(int num)
    {
        int trueIndex = num < 0 ? num + Effect.Targets.Count : num;
        return trueIndex < 0 ? null : Effect.Targets[trueIndex];
    }

    public Card Target => GetTarget(TargetIndex);
}
