﻿using KompasCore.Cards;
using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class SwapSubeffect : ServerSubeffect
    {
        public int SecondTargetIndex = -2;
        public GameCard SecondTarget => Effect.GetTarget(SecondTargetIndex);
        public override bool IsImpossible() => CardTarget == null || SecondTarget == null;

        public override Task<ResolutionInfo> Resolve()
        {
            if (CardTarget == null)
                throw new NullCardException(TargetWasNull);
            else if (forbidNotBoard && CardTarget.Location != CardLocation.Field)
                throw new InvalidLocationException(CardTarget.Location, CardTarget, MovedCardOffBoard);

            if (SecondTarget == null)
                throw new NullCardException(TargetWasNull);
            else if (forbidNotBoard && SecondTarget.Location != CardLocation.Field)
                throw new InvalidLocationException(SecondTarget.Location, SecondTarget, MovedCardOffBoard);

            CardTarget.Move(SecondTarget.Position, false, ServerEffect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}