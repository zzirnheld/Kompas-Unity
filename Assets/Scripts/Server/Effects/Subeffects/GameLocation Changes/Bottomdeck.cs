﻿using KompasCore.Cards.Movement;
using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class Bottomdeck : CardChangeState
    {
        public override bool IsImpossible() => CardTarget == null;
        protected override CardLocation destination => CardLocation.Deck;

        public override Task<ResolutionInfo> Resolve()
        {
            if (CardTarget == null) throw new NullCardException(TargetWasNull);

            CardTarget.Bottomdeck(CardTarget.Owner, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}