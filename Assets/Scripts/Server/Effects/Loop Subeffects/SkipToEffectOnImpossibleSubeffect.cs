using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Resolves a specified subeffect if at any point the effect is declared impossible
/// </summary>
public class SkipToEffectOnImpossibleSubeffect : ServerSubeffect
{
    public int JumpTo;

    public override void Resolve()
    {
        ServerEffect.OnImpossible = this;
        ServerEffect.ResolveNextSubeffect();
    }

    public override void OnImpossible()
    {
        //forget about this effect on impossible, and jump to a new one
        ServerEffect.OnImpossible = null;
        ServerEffect.ResolveSubeffect(JumpTo);
    }
}
