using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Swaps two values among one card's own NESW. E for W, for example.
/// </summary>
public class SwapOwnNESWSubeffect : Subeffect
{
    public int Stat1;
    public int Stat2;

    public override void Resolve()
    {
        var charTarget = Target as CharacterCard;
        int[] newStats = { charTarget.N, charTarget.E, charTarget.S, charTarget.W };
        (newStats[Stat1], newStats[Stat2]) = (newStats[Stat2], newStats[Stat1]);
        ServerGame.SetStats(charTarget, newStats);

        Effect.ResolveNextSubeffect();
    }
}
