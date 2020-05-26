using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryActivationSubeffect : TemporarySubeffect
{
    public override void Resolve()
    {
        var tempActivation = new HangingActivationEffect(ServerGame, TriggerRestriction, EndCondition,
            Target, this);
        ServerEffect.ResolveNextSubeffect();
    }
}
