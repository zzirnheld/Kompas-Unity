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

        var char1 = target1 as CharacterCard ?? throw new System.ArgumentException($"Target {target1.CardName} is not a character");
        var char2 = target2 as CharacterCard ?? throw new System.ArgumentException($"Target {target2.CardName} is not a character");

        ServerGame.SwapStats(char1, char2, SwapN, SwapE, SwapS, SwapW);
    }
}
