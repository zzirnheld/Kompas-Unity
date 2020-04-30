using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetXSubeffect : ServerSubeffect
{
    public override void Resolve()
    {
        EffectController.ServerNotifier.GetXForEffect(ThisCard, ServerEffect.EffectIndex, ServerEffect.subeffectIndex);
    }
}
