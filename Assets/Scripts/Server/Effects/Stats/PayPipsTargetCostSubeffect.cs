using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class PayPipsTargetCostSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));

            int toPay = Target.Cost * xMultiplier / xDivisor + xModifier;
            if (ServerPlayer.Pips < toPay) return Task.FromResult(ResolutionInfo.Impossible(CantAffordPips));
            else
            {
                Debug.Log("Paying " + toPay + " pips for target cost");
                ServerGame.GivePlayerPips(ServerPlayer, ServerPlayer.Pips - toPay);
                return Task.FromResult(ResolutionInfo.Next);
            }
        }
    }
}