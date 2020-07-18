using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTriggeringCardSubeffect : ServerSubeffect
{
    public override bool Resolve()
    {
        if (ServerEffect.CurrActivationContext.Card == null)
            return ServerEffect.EffectImpossible();

        ServerEffect.AddTarget(ServerEffect.CurrActivationContext.Card);
        return ServerEffect.ResolveNextSubeffect();
    }
}
