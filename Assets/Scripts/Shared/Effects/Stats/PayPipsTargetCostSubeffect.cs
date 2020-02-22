using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayPipsTargetCostSubeffect : Subeffect
{
    public override void Resolve()
    {
        int toPay = Target.Cost;
        if (parent.EffectController.pips < toPay)
        {
            parent.EffectImpossible();
            return;
        }

        Debug.Log("Paying " + toPay + " pips for target cost");
        parent.EffectController.pips -= toPay;
        parent.EffectController.ServerNotifier.NotifySetPips(parent.EffectController.pips);
        parent.ResolveNextSubeffect();
    }
}
