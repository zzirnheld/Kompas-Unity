using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopdeckSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        if (Target == null)
        {
            ServerEffect.EffectImpossible();
            return;
        }
        ServerGame.Topdeck(Target, ServerEffect);
        ServerEffect.ResolveNextSubeffect();
    }
}
