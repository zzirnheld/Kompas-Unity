using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Subeffect
{
    [System.NonSerialized] public Effect Effect;

    public ServerGame ServerGame { get { return Effect.serverGame; } }
    public ServerPlayer EffectController { get { return Effect.EffectController; } }
    public Card ThisCard { get { return Effect.thisCard; } }

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
        Effect.targets.Add(card);
        Effect.ResolveSubeffect(Effect.subeffectIndex + 1);
    }

    public virtual void Initialize() { }

    public virtual void OnImpossible() { }

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
                Effect.targets[Effect.targets.Count + TargetIndex] :
                Effect.targets[TargetIndex];
        }
    }
}