using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Resolves a specified subeffect if at any point the effect is declared impossible
/// </summary>
public class SkipToEffectOnImpossibleSubeffect : Subeffect
{
    public int JumpTo;

    public override void Resolve()
    {
        Effect.OnImpossible = this;
        Effect.ResolveNextSubeffect();
    }

    public override void OnImpossible()
    {
        //forget about this effect on impossible, and jump to a new one
        Effect.OnImpossible = null;
        Effect.ResolveSubeffect(JumpTo);
    }
}
