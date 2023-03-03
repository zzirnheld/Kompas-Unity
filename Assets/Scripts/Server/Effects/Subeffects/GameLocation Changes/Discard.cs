﻿using KompasCore.Cards.Movement;
using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class Discard : CardChangeState
    {
        protected override CardLocation destination => CardLocation.Discard;

        public override Task<ResolutionInfo> Resolve()
        {
            if (CardTarget == null) throw new NullCardException(TargetWasNull);

            CardTarget.Discard(ServerEffect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}