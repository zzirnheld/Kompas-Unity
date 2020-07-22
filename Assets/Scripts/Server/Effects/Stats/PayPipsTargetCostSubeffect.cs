using UnityEngine;

namespace KompasServer.Effects
{
    public class PayPipsTargetCostSubeffect : ServerSubeffect
    {
        public int multiplier = 1;
        public int divisor = 1;
        public int modifier = 0;

        public override bool Resolve()
        {
            if (Target == null) return ServerEffect.EffectImpossible();

            int toPay = Target.Cost * multiplier / divisor + modifier;
            if (EffectController.Pips < toPay) return ServerEffect.EffectImpossible();
            else
            {
                Debug.Log("Paying " + toPay + " pips for target cost");
                ServerGame.GivePlayerPips(EffectController, EffectController.Pips - toPay);
                return ServerEffect.ResolveNextSubeffect();
            }
        }
    }
}