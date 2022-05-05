﻿using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetTriggeringStackableSourceSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Context.stackableCause == null) throw new KompasException("Null stackable", string.Empty);

            ServerEffect.AddTarget(Context.stackableCause.Source);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}