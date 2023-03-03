using KompasCore.Exceptions;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects.Subeffect
{
    public class PayPipsTargetCostSubeffect : ServerSubeffect
    {
        public int ToPay => CardTarget.Cost * xMultiplier / xDivisor + xModifier;

        public override bool IsImpossible() => CardTarget == null || ServerPlayer.Pips < ToPay;

        public override Task<ResolutionInfo> Resolve()
        {
            if (CardTarget == null) throw new NullCardException(TargetWasNull);

            if (ServerPlayer.Pips < ToPay) return Task.FromResult(ResolutionInfo.Impossible(CantAffordPips));
            else
            {
                Debug.Log($"Paying {ToPay} pips for target cost");
                ServerPlayer.Pips -= ToPay;
                return Task.FromResult(ResolutionInfo.Next);
            }
        }
    }
}