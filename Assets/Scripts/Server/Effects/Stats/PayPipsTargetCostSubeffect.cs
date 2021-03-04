using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class PayPipsTargetCostSubeffect : ServerSubeffect
    {
        public int ToPay => Target.Cost * xMultiplier / xDivisor + xModifier;

        public override bool IsImpossible() => Target == null || ServerPlayer.Pips < ToPay;

        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));

            if (ServerPlayer.Pips < ToPay) return Task.FromResult(ResolutionInfo.Impossible(CantAffordPips));
            else
            {
                Debug.Log($"Paying {ToPay} pips for target cost");
                ServerGame.GivePlayerPips(ServerPlayer, ServerPlayer.Pips - ToPay);
                return Task.FromResult(ResolutionInfo.Next);
            }
        }
    }
}