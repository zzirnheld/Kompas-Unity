using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RehandSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        if (Target == null)
        {
            ServerEffect.EffectImpossible();
            return;
        }
        ServerEffect.serverGame.Rehand(Target, ServerEffect);
        ServerEffect.ResolveNextSubeffect();
    }
}
