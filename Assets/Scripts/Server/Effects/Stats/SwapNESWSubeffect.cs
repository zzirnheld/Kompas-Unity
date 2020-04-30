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

    public override void Resolve()
    {
        var target1 = TargetIndices[0] < 0 ?
                ServerEffect.targets[ServerEffect.targets.Count + TargetIndices[0]] :
                ServerEffect.targets[TargetIndices[0]];
        var target2 = TargetIndices[1] < 0 ?
                ServerEffect.targets[ServerEffect.targets.Count + TargetIndices[1]] :
                ServerEffect.targets[TargetIndices[1]];

        if(!(target1 is CharacterCard && target2 is CharacterCard))
        {
            throw new System.ArgumentException($"Targets {target1.CardName} and {target2.CardName} are not both characters");
        }

        ServerGame.SwapStats(target1 as CharacterCard, target2 as CharacterCard, SwapN, SwapE, SwapS, SwapW);
    }
}
