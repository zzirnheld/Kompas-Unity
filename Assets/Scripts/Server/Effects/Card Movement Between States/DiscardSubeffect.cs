using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        if (Target == null) ServerEffect.EffectImpossible();
        else
        {
            if (Target.Discard(ServerEffect)) ServerEffect.ResolveNextSubeffect();
            else ServerEffect.EffectImpossible();
        }
    }
}
