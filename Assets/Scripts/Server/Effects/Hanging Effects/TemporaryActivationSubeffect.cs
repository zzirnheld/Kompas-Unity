using System.Collections.Generic;

public class TemporaryActivationSubeffect : TemporaryCardChangeSubeffect
{
    protected override IEnumerable<(HangingEffect, GameCard)> CreateHangingEffects()
    {
        var tempActivation = new HangingActivationEffect(ServerGame, triggerRestriction, endCondition,
            Target, this);
        return new List<(HangingEffect, GameCard)>() { (tempActivation, Target) };
    }
}
