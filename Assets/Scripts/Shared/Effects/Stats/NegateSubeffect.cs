using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NegateSubeffect : Subeffect
{
    public override void Resolve()
    {
        if(Target == null)
        {
            Effect.EffectImpossible();
            return;
        }

        ServerGame.Negate(Target);
        Effect.ResolveNextSubeffect();
    }
}
