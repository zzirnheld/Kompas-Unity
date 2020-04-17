using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayPipsSubeffect : Subeffect
{
    public int xMultiplier = 1;
    public int xDivisor = 1;
    public int modifier = 0;

    public override void Resolve()
    {
        int toPay = Effect.X * xMultiplier / xDivisor + modifier;
        if(EffectController.pips < toPay)
        {
            Effect.EffectImpossible();
            return;
        }

        EffectController.pips -= toPay;
        EffectController.ServerNotifier.NotifySetPips(EffectController.pips);
        Effect.ResolveNextSubeffect();
    }
}
