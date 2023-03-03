﻿using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class TargetTargetsAugmentedCard : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (CardTarget == null) throw new NullCardException(NoValidCardTarget);
            else if (!CardTarget.Attached) throw new NullCardException(NoValidCardTarget);
            else
            {
                ServerEffect.AddTarget(CardTarget.AugmentedCard);
                return Task.FromResult(ResolutionInfo.Next);
            }
        }
    }
}