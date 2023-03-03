﻿using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class Reveal : ServerSubeffect
    {
        public override bool IsImpossible() => CardTarget == null || CardTarget.KnownToEnemy;

        public override Task<ResolutionInfo> Resolve()
        {
            if (CardTarget == null) throw new NullCardException(TargetWasNull);

            CardTarget.Reveal(Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}