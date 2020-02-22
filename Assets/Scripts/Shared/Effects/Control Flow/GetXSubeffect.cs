using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetXSubeffect : Subeffect
{
    public override void Resolve()
    {
        parent.EffectController.ServerNotifier
            .GetXForEffect(parent.thisCard, parent.EffectIndex, parent.subeffectIndex);
    }
}
