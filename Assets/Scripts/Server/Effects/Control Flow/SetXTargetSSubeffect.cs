using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetXTargetSSubeffect : ServerSubeffect
{
    public override void Resolve()
    {
        if (!(Target is CharacterCard charTarget))
        {
            ServerEffect.EffectImpossible();
            return;
        }

        ServerEffect.X = charTarget.S;
        EffectController.ServerNotifier.NotifyEffectX(ThisCard, ServerEffect.EffectIndex, ServerEffect.X);
        ServerEffect.ResolveNextSubeffect();
    }
}
