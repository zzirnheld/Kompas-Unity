using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopdeckSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        if (Target.Topdeck(Target.Owner, Effect)) ServerEffect.ResolveNextSubeffect();
        else ServerEffect.EffectImpossible();
    }
}
