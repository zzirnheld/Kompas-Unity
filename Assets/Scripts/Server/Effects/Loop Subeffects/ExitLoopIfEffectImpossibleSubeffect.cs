using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitLoopIfEffectImpossibleSubeffect : ServerSubeffect
{
    public int LoopSubeffectIndex;

    private LoopSubeffect loopSubeffect;

    public override void Initialize(ServerEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        loopSubeffect = ServerEffect.ServerSubeffects[LoopSubeffectIndex] as LoopSubeffect 
            ?? throw new System.ArgumentNullException($"Subeffect at loop subeffect index {LoopSubeffectIndex} cannot be null");
    }

    public override void Resolve()
    {
        ServerEffect.OnImpossible = this;
    }

    public override void OnImpossible()
    {
        loopSubeffect.ExitLoop();
    }
}
