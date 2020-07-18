using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Swaps two values among one card's own NESW. E for W, for example.
/// </summary>
public class SwapOwnNESWSubeffect : ServerSubeffect
{
    public int Stat1;
    public int Stat2;

    public override bool Resolve()
    {
        if (Target == null) return ServerEffect.EffectImpossible();

        int[] newStats = { Target.N, Target.E, Target.S, Target.W };
        (newStats[Stat1], newStats[Stat2]) = (newStats[Stat2], newStats[Stat1]);
        Target.SetCharStats(newStats[0], newStats[1], newStats[2], newStats[3]);

        return ServerEffect.ResolveNextSubeffect();
    }
}
