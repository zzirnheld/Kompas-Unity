using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomdeckSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        if (Target == null) ServerEffect.EffectImpossible();
        else
        {
            if (Target.Bottomdeck(Target.Owner, Effect)) ServerEffect.ResolveNextSubeffect();
            else ServerEffect.EffectImpossible();
        }
    }
}
