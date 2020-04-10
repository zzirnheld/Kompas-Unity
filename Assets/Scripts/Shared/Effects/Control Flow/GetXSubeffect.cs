using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetXSubeffect : Subeffect
{
    public override void Resolve()
    {
        EffectController.ServerNotifier.GetXForEffect(ThisCard, Effect.EffectIndex, Effect.subeffectIndex);
    }
}
