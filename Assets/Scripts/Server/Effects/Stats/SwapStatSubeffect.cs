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

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            firstTargetStat?.Initialize(eff.Source);
            secondTargetStat?.Initialize(eff.Source);
        }

        public override Task<ResolutionInfo> Resolve()
        {
            var secondTarget = Effect.GetTarget(secondTargetIndex);
            if (CardTarget == null)
                throw new NullCardException(TargetWasNull);
            else if (forbidNotBoard && CardTarget.Location != CardLocation.Field)
                throw new InvalidLocationException(CardTarget.Location, CardTarget, ChangedStatsOfCardOffBoard);

            if (secondTarget == null)
                throw new NullCardException(TargetWasNull);
            else if (forbidNotBoard && CardTarget.Location != CardLocation.Field)
                throw new InvalidLocationException(secondTarget.Location, secondTarget, ChangedStatsOfCardOffBoard);

            var firstStat = firstTargetStat.GetValueOf(CardTarget);
            var secondStat = firstTargetStat.GetValueOf(secondTarget);
            firstTargetStat.SetValueOf(secondTarget, firstStat, Effect);
            secondTargetStat.SetValueOf(CardTarget, secondStat, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}