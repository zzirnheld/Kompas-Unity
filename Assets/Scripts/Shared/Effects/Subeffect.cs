using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Subeffect
{
    [System.NonSerialized] public Effect parent;

    public ServerGame ServerGame { get { return parent.serverGame; } }

    public int SubeffIndex;

    /// <summary>
    /// parent resolve method. at the end, needs to call resolve subeffect in parent
    /// if it's an if, it does a specific index
    /// otherwise, it does currentIndex + 1
    /// </summary>
    public abstract void Resolve();

    /// <summary>
    /// Called by restrictions that have found a valid target to add to the list
    /// </summary>
    /// <param name="card"></param>
    public void ContinueResolutionWith(Card card)
    {
        parent.targets.Add(card);
        parent.ResolveSubeffect(parent.subeffectIndex + 1);
    }

    public virtual void Initialize()
    {

    }

    /// <summary>
    /// The index in the Effect.targets array for which target this effect uses.
    /// If positive, just an index.
    /// If negative, it's Effect.targets.Count + targetIndex (aka that many back from the end)
    /// </summary>
    public int TargetIndex;

    public Card Target
    {
        get
        {
            return TargetIndex < 0 ?
                parent.targets[parent.targets.Count + TargetIndex] :
                parent.targets[TargetIndex];
        }
    }
}