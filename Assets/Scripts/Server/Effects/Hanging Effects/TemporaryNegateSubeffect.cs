using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryNegateSubeffect : TemporarySubeffect
{
    public override void Resolve()
    {
        var tempNegation = new HangingNegationEffect(ServerGame, TriggerRestriction, EndCondition,
            Target, this);
        ServerEffect.ResolveNextSubeffect();
    }
}
