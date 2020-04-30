using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomdeckSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        if (Target == null)
        {
            Effect.EffectImpossible();
            return;
        }
        ServerGame.Bottomdeck(Target, Effect);
        Effect.ResolveNextSubeffect();
    }
}
