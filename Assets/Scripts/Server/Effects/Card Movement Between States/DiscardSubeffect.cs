using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardSubeffect : CardChangeStateSubeffect
{
    public override bool Resolve()
    {
        if (Target != null && Target.Discard(ServerEffect))
            return ServerEffect.ResolveNextSubeffect();
        else return ServerEffect.EffectImpossible();
    }
}
