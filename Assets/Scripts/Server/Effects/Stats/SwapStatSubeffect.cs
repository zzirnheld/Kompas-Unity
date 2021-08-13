using KompasCore.Effects;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class SwapStatSubeffect : ServerSubeffect
    {
        public CardValue firstTargetStat;
        public CardValue secondTargetStat;
        public int secondTargetIndex = -2;

        public override Task<ResolutionInfo> Resolve()
        {
            var secondTarget = Effect.GetTarget(secondTargetIndex);
            if (Target == null || secondTarget == null) 
                return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));
            else if (Target.Location != CardLocation.Field || secondTarget.Location != CardLocation.Field)
                return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

            var firstStat = firstTargetStat.GetValueOf(Target);
            var secondStat = firstTargetStat.GetValueOf(secondTarget);
            firstTargetStat.SetValueOf(secondTarget, firstStat, Effect);
            secondTargetStat.SetValueOf(Target, secondStat, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}