using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RehandSubeffect : CardChangeStateSubeffect
{
    public override bool Resolve()
    {
        if (Target != null && Target.Rehand(Target.Owner, Effect)) 
            return ServerEffect.ResolveNextSubeffect();
        else return ServerEffect.EffectImpossible();
    }
}
