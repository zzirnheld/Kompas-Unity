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

    public int SubeffIndex { get; private set; }

    /// <summary>
    /// parent resolve method. at the end, needs to call resolve subeffect in parent
    /// if it's an if, it does a specific index
    /// otherwise, it does currentIndex + 1
    /// </summary>
    public abstract void Resolve();

    public virtual void Initialize(Effect eff, int subeffIndex) {
        this.Effect = eff;
        this.SubeffIndex = subeffIndex;
    }

    /// <summary>
    /// Optional method. If implemented, does something when the effect is declared impossible.
    /// Default implementation just finishes resolution of the effect
    /// </summary>
    public virtual void OnImpossible() {
        Debug.Log($"On Impossible called for {GetType()} without an override");
        Effect.ResolveSubeffect(Effect.Subeffects.Length);
    }

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
            int trueIndex = TargetIndex < 0 ? TargetIndex + Effect.targets.Count : TargetIndex;
            return trueIndex < 0 ? null : Effect.targets[trueIndex];
        }
    }
}