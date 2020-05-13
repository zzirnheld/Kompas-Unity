using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Subeffect
{
    public abstract Effect Effect { get; }
    public abstract Player Controller { get; }

    public int SubeffIndex { get; protected set; }

    public Card Source => Effect.Source;

    /// <summary>
    /// The index in the Effect.targets array for which target this effect uses.
    /// If positive, just an index.
    /// If negative, it's Effect.targets.Count + targetIndex (aka that many back from the end)
    /// </summary>
    public int TargetIndex = -1;

    public Card Target
    {
        get
        {
            int trueIndex = TargetIndex < 0 ? TargetIndex + Effect.Targets.Count : TargetIndex;
            return trueIndex < 0 ? null : Effect.Targets[trueIndex];
        }
    }
}
