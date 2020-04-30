using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NegateSubeffect : ServerSubeffect
{
    public override void Resolve()
    {
        if(Target == null)
        {
            ServerEffect.EffectImpossible();
            return;
        }

        ServerGame.Negate(Target);
        ServerEffect.ResolveNextSubeffect();
    }
}
