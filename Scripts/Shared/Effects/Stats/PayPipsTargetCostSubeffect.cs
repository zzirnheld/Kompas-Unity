using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayPipsTargetCostSubeffect : Subeffect
{
    public override void Resolve()
    {
        int toPay = Target.GetCost();
        if (parent.EffectController.pips < toPay)
        {
            parent.EffectImpossible();
            return;
        }

        Debug.Log("Paying " + toPay + " pips for target cost");
        parent.EffectController.pips -= toPay;
        ServerGame.serverNetworkCtrl.NotifySetPips(ServerGame, parent.effectControllerIndex, parent.EffectController.pips, parent.EffectController.ConnectionID);
        parent.ResolveNextSubeffect();
    }
}
