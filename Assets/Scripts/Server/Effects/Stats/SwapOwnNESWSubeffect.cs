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
        if(!(Target is CharacterCard charCard))
        {
            Effect.EffectImpossible();
            return;
        }

        int[] newStats = { charCard.N, charCard.E, charCard.S, charCard.W };
        (newStats[Stat1], newStats[Stat2]) = (newStats[Stat2], newStats[Stat1]);
        ServerGame.SetStats(charCard, newStats);

        Effect.ResolveNextSubeffect();
    }
}
