using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayPipsTargetCostSubeffect : ServerSubeffect
{
    public int Multiplier = 1;
    public int Divisor = 1;
    public int Modifier = 0;

    public override bool Resolve()
    {
        if(Target == null) return ServerEffect.EffectImpossible();

        int toPay = Target.Cost * Multiplier / Divisor + Modifier;
        if (EffectController.Pips < toPay) return ServerEffect.EffectImpossible();
        else
        {
            Debug.Log("Paying " + toPay + " pips for target cost");
            ServerGame.GivePlayerPips(EffectController, EffectController.Pips - toPay);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}
