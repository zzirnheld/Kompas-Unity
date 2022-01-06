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

        public int RealNVal => nVal < 0 ? CardTarget.N : nVal;
        public int RealEVal => eVal < 0 ? CardTarget.E : eVal;
        public int RealSVal => sVal < 0 ? CardTarget.S : sVal;
        public int RealWVal => wVal < 0 ? CardTarget.W : wVal;
        public int RealCVal => cVal < 0 ? CardTarget.C : cVal;
        public int RealAVal => aVal < 0 ? CardTarget.A : aVal;

        public override Task<ResolutionInfo> Resolve()
        {
            if (CardTarget == null)
                throw new NullCardException(TargetWasNull);
            else if (forbidNotBoard && CardTarget.Location != CardLocation.Field)
                throw new InvalidLocationException(CardTarget.Location, CardTarget, ChangedStatsOfCardOffBoard);

            CardTarget.SetStats((RealNVal, RealEVal, RealSVal, RealWVal, RealCVal, RealAVal), Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}