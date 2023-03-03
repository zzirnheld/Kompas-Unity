using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class SetCardStatsSubeffect : ServerSubeffect
    {
        public int nVal = -1;
        public int eVal = -1;
        public int sVal = -1;
        public int wVal = -1;
        public int cVal = -1;
        public int aVal = -1;

        private CardStats Stats => new CardStats(
            nVal < 0 ? CardTarget.N : nVal,
            eVal < 0 ? CardTarget.E : eVal,
            sVal < 0 ? CardTarget.S : sVal,
            wVal < 0 ? CardTarget.W : wVal,
            cVal < 0 ? CardTarget.C : cVal,
            aVal < 0 ? CardTarget.A : aVal
        );

        public override Task<ResolutionInfo> Resolve()
        {
            if (CardTarget == null)
                throw new NullCardException(TargetWasNull);
            else if (forbidNotBoard && CardTarget.Location != CardLocation.Board)
                throw new InvalidLocationException(CardTarget.Location, CardTarget, ChangedStatsOfCardOffBoard);

            CardTarget.SetStats(Stats, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}