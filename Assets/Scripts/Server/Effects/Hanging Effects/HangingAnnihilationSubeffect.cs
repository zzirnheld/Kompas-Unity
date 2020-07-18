using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangingAnnihilationSubeffect : TemporaryCardChangeSubeffect
{
    protected override IEnumerable<(HangingEffect, GameCard)> CreateHangingEffects()
    {
        var eff = new HangingAnnihilationEffect(ServerGame, triggerRestriction, endCondition, Target);
        return new List<(HangingEffect, GameCard)>() { (eff, Target) };
    }
}
