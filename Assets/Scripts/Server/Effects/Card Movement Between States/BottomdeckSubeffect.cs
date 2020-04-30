using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomdeckSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        if (Target == null)
        {
            ServerEffect.EffectImpossible();
            return;
        }
        ServerGame.Bottomdeck(Target, ServerEffect);
        ServerEffect.ResolveNextSubeffect();
    }
}
