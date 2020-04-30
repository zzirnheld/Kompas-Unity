using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RehandSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        if (Target == null)
        {
            Effect.EffectImpossible();
            return;
        }
        Effect.serverGame.Rehand(Target, Effect);
        Effect.ResolveNextSubeffect();
    }
}
