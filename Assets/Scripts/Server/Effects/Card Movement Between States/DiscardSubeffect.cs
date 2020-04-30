using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        if (Target == null)
        {
            ServerEffect.EffectImpossible();
            return;
        }
        Debug.Log("Resolving discard subeffect");
        ServerGame.Discard(Target, ServerEffect);
        ServerEffect.ResolveNextSubeffect();
    }
}
