using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapNESWSubeffect : ServerSubeffect
{
    public int[] targetIndices;

    public bool swapN = false;
    public bool swapE = false;
    public bool swapS = false;
    public bool swapW = false;

    public override bool Resolve()
    {
        var target1 = targetIndices[0] < 0 ?
                ServerEffect.Targets[ServerEffect.Targets.Count + targetIndices[0]] :
                ServerEffect.Targets[targetIndices[0]];
        var target2 = targetIndices[1] < 0 ?
                ServerEffect.Targets[ServerEffect.Targets.Count + targetIndices[1]] :
                ServerEffect.Targets[targetIndices[1]];
        if (target1 == null || target2 == null) return ServerEffect.EffectImpossible();

        target1.SwapCharStats(target2, swapN, swapE, swapS, swapW);
        return ServerEffect.ResolveNextSubeffect();
    }
}
