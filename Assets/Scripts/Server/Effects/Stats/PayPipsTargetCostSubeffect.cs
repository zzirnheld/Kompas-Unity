using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayPipsTargetCostSubeffect : ServerSubeffect
{
    public override void Resolve()
    {
        if(Target == null)
        {
            ServerEffect.EffectImpossible();
            return;
        }

        int toPay = Target.Cost;
        if (EffectController.pips < toPay)
        {
            ServerEffect.EffectImpossible();
            return;
        }

        Debug.Log("Paying " + toPay + " pips for target cost");
        EffectController.pips -= toPay;
        EffectController.ServerNotifier.NotifySetPips(EffectController.pips);
        ServerEffect.ResolveNextSubeffect();
    }
}
