﻿using System.Threading.Tasks;
using KompasCore.Effects.Identities;
using KompasCore.Effects.Identities.ActivationContextNumberIdentities;

namespace KompasServer.Effects.Subeffects
{
    public class PayPips : ServerSubeffect
    {
        public override bool IsImpossible() => PlayerTarget.Pips < ToPay;

        private int ToPay => pipCost.From(CurrentContext, default);

        public IIdentity<int> pipCost = new X();

        public override Task<ResolutionInfo> Resolve()
        {
            int toPay = ToPay;
            if (PlayerTarget.Pips < toPay) return Task.FromResult(ResolutionInfo.Impossible(CantAffordPips));

            PlayerTarget.Pips -= toPay;
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}