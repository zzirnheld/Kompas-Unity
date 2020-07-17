using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RehandSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        if (Target == null) ServerEffect.EffectImpossible();
        else
        {
            if (Target.Rehand(Target.Owner, Effect)) ServerEffect.ResolveNextSubeffect();
            else ServerEffect.EffectImpossible();
        }
    }
}
