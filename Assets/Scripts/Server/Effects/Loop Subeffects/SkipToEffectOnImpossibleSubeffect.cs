using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Resolves a specified subeffect if at any point the effect is declared impossible
/// </summary>
public class SkipToEffectOnImpossibleSubeffect : ServerSubeffect
{
    public int jumpTo;

    public override bool Resolve()
    {
        ServerEffect.OnImpossible = this;
        return ServerEffect.ResolveNextSubeffect();
    }

    public override bool OnImpossible()
    {
        //forget about this effect on impossible, and jump to a new one
        ServerEffect.OnImpossible = null;
        return ServerEffect.ResolveSubeffect(jumpTo);
    }
}
