using KompasCore.Exceptions;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class DamageSubeffect : ServerSubeffect
    {
        public override bool IsImpossible() => Target == null;

        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null)
                throw new NullCardException(TargetWasNull);
            else if (forbidNotBoard && Target.Location != CardLocation.Field)
                throw new InvalidLocationException(Target.Location, Target, ChangedStatsOfCardOffBoard);

            Target.TakeDamage(Count, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}