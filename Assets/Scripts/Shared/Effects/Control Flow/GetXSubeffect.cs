using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetXSubeffect : Subeffect
{
    public override void Resolve()
    {
        ServerGame.serverNotifier.GetXForEffect(parent.EffectController, parent.thisCard, parent.EffectIndex, parent.subeffectIndex);
    }
}
