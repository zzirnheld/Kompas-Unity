using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReshuffleSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        if (Target == null) ServerEffect.EffectImpossible();
        else
        {
            if (Target.Reshuffle(Target.Owner, Effect)) ServerEffect.ResolveNextSubeffect();
            else ServerEffect.EffectImpossible();
        }
    }
}
