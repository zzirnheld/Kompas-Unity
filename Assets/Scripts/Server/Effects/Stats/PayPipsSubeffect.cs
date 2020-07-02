using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayPipsSubeffect : ServerSubeffect
{
    public int xMultiplier = 1;
    public int xDivisor = 1;
    public int modifier = 0;

    public override void Resolve()
    {
        int toPay = ServerEffect.X * xMultiplier / xDivisor + modifier;
        if(EffectController.Pips < toPay)
        {
            ServerEffect.EffectImpossible();
            return;
        }

        EffectController.Pips -= toPay;
        ServerEffect.ResolveNextSubeffect();
    }
}
