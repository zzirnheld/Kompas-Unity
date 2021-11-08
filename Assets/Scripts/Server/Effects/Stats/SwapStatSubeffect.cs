using KompasCore.Effects;
using KompasCore.Exceptions;
using System.Threading.Tasks;

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
            if (Target == null)
                throw new NullCardException(TargetWasNull);
            else if (forbidNotBoard && Target.Location != CardLocation.Field)
                throw new InvalidLocationException(Target.Location, Target, ChangedStatsOfCardOffBoard);

            if (secondTarget == null)
                throw new NullCardException(TargetWasNull);
            else if (forbidNotBoard && Target.Location != CardLocation.Field)
                throw new InvalidLocationException(secondTarget.Location, secondTarget, ChangedStatsOfCardOffBoard);

            var firstStat = firstTargetStat.GetValueOf(Target);
            var secondStat = firstTargetStat.GetValueOf(secondTarget);
            firstTargetStat.SetValueOf(secondTarget, firstStat, Effect);
            secondTargetStat.SetValueOf(Target, secondStat, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}