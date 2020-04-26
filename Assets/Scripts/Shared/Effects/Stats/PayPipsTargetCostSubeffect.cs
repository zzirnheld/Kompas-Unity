using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayPipsTargetCostSubeffect : Subeffect
{
    public override void Resolve()
    {
        if(Target == null)
        {
            Effect.EffectImpossible();
            return;
        }

        int toPay = Target.Cost;
        if (EffectController.pips < toPay)
        {
            Effect.EffectImpossible();
            return;
        }

        Debug.Log("Paying " + toPay + " pips for target cost");
        EffectController.pips -= toPay;
        EffectController.ServerNotifier.NotifySetPips(EffectController.pips);
        Effect.ResolveNextSubeffect();
    }
}
