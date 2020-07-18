using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryNegateSubeffect : TemporaryCardChangeSubeffect
{
    protected override IEnumerable<(HangingEffect, GameCard)> CreateHangingEffects()
    {
        var tempNegation = new HangingNegationEffect(ServerGame, triggerRestriction, endCondition,
            Target, this);
        return new List<(HangingEffect, GameCard)>() { (tempNegation, Target) };
    }
}
