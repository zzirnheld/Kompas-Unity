using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetXTargetSSubeffect : ServerSubeffect
{
    public override void Resolve()
    {
        ServerEffect.X = Target.S;
        EffectController.ServerNotifier.NotifyEffectX(ThisCard, ServerEffect.EffectIndex, ServerEffect.X);
        ServerEffect.ResolveNextSubeffect();
    }
}
