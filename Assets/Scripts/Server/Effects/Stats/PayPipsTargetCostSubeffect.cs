using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayPipsTargetCostSubeffect : ServerSubeffect
{
    public int Multiplier = 1;
    public int Divisor = 1;
    public int Modifier = 0;

    public override void Resolve()
    {
        if(Target == null)
        {
            ServerEffect.EffectImpossible();
            return;
        }

        int toPay = Target.Cost * Multiplier / Divisor + Modifier;
        if (EffectController.Pips < toPay) ServerEffect.EffectImpossible();
        else
        {
            Debug.Log("Paying " + toPay + " pips for target cost");
            ServerGame.GivePlayerPips(EffectController, EffectController.Pips - toPay);
            ServerEffect.ResolveNextSubeffect();
        }
    }
}
