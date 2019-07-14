using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves cards between discard/field/etc
/// </summary>
public abstract class CardChangeStateSubeffect : Subeffect
{
    /// <summary>
    /// The index in the Effect.targets array for which target this effect uses.
    /// If positive, just an index.
    /// If negative, it's Effect.targets.Count + targetIndex (aka that many back from the end)
    /// </summary>
    public int TargetIndex;

    protected Card Target
    {
        get
        {
            return TargetIndex < 0 ?
                parent.targets[parent.targets.Count + TargetIndex] :
                parent.targets[TargetIndex];
        }
    }
}
