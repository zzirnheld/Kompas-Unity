using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopdeckSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        if (Target == null)
        {
            Effect.EffectImpossible();
            return;
        }
        ServerGame.Topdeck(Target, Effect);
        Effect.ResolveNextSubeffect();
    }
}
