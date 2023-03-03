using KompasCore.Exceptions;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects.Subeffects
{
    public class Damage : ServerSubeffect
    {
        public override bool IsImpossible() => CardTarget == null;

        public override Task<ResolutionInfo> Resolve()
        {
            if (CardTarget == null)
                throw new NullCardException(TargetWasNull);
            else if (forbidNotBoard && CardTarget.Location != CardLocation.Board)
                throw new InvalidLocationException(CardTarget.Location, CardTarget, ChangedStatsOfCardOffBoard);

            CardTarget.TakeDamage(Count, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}