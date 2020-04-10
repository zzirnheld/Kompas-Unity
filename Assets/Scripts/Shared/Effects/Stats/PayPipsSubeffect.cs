using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayPipsSubeffect : Subeffect
{
    public int xMultiplier;
    public int xDivisor;
    public int modifier;

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
