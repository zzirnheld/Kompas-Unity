using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitLoopIfEffectImpossibleSubeffect : Subeffect
{
    public int LoopSubeffectIndex;

    private LoopSubeffect loopSubeffect;

    public override void Initialize()
    {
        base.Initialize();
        loopSubeffect = parent.Subeffects[LoopSubeffectIndex] as LoopSubeffect 
            ?? throw new System.ArgumentNullException($"Subeffect at loop subeffect index {LoopSubeffectIndex} cannot be null");
    }

    public override void Resolve()
    {
        parent.OnImpossible = this;
    }

    public override void OnImpossible()
    {
        loopSubeffect.ExitLoop();
    }
}
