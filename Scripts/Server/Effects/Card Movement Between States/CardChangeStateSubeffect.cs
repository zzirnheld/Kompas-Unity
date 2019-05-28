using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardChangeStateSubeffect : Subeffect
{
    /// <summary>
    /// The index in the Effect.targets array for which target this effect uses.
    /// If positive, just an index.
    /// If negative, it's Effect.targets.Count + targetIndex (aka that many back from the end)
    /// </summary>
    public int TargetIndex;

    protected Card GetTarget()
    {
        if (TargetIndex < 0) return parent.targets[parent.targets.Count + TargetIndex];
        else return parent.targets[TargetIndex];
    }
}
