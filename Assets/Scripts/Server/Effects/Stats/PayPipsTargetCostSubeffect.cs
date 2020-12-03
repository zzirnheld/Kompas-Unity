using UnityEngine;

namespace KompasServer.Effects
{
    public class PayPipsTargetCostSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            if (Target == null) return ServerEffect.EffectImpossible();

            int toPay = Target.Cost * xMultiplier / xDivisor + xModifier;
            if (ServerPlayer.Pips < toPay) return ServerEffect.EffectImpossible();
            else
            {
                Debug.Log("Paying " + toPay + " pips for target cost");
                ServerGame.GivePlayerPips(ServerPlayer, ServerPlayer.Pips - toPay);
                return ServerEffect.ResolveNextSubeffect();
            }
        }
    }
}