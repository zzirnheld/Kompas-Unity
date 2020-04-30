using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetXTargetSSubeffect : Subeffect
{
    public override void Resolve()
    {
        if (!(Target is CharacterCard charTarget))
        {
            Effect.EffectImpossible();
            return;
        }

        Effect.X = charTarget.S;
        EffectController.ServerNotifier.NotifyEffectX(ThisCard, Effect.EffectIndex, Effect.X);
        Effect.ResolveNextSubeffect();
    }
}
