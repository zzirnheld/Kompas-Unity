﻿using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class TargetAugmentedCard : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (!ServerEffect.Source.Attached) throw new NullCardException(NoValidCardTarget);
            else
            {
                ServerEffect.AddTarget(Source.AugmentedCard);
                return Task.FromResult(ResolutionInfo.Next);
            }
        }
    }
}