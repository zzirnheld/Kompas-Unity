using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapNESWSubeffect : ServerSubeffect
{
    public int[] TargetIndices;

    public bool SwapN = false;
    public bool SwapE = false;
    public bool SwapS = false;
    public bool SwapW = false;

    public override bool Resolve()
    {
        var target1 = TargetIndices[0] < 0 ?
                ServerEffect.Targets[ServerEffect.Targets.Count + TargetIndices[0]] :
                ServerEffect.Targets[TargetIndices[0]];
        var target2 = TargetIndices[1] < 0 ?
                ServerEffect.Targets[ServerEffect.Targets.Count + TargetIndices[1]] :
                ServerEffect.Targets[TargetIndices[1]];
        if (target1 == null || target2 == null) return ServerEffect.EffectImpossible();

        target1.SwapCharStats(target2, SwapN, SwapE, SwapS, SwapW);
        return ServerEffect.ResolveNextSubeffect();
    }
}
