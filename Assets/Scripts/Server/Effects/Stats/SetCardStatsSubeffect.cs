using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class SetCardStatsSubeffect : ServerSubeffect
    {
        public bool forbidNotBoard = true;

        public int nVal = -1;
        public int eVal = -1;
        public int sVal = -1;
        public int wVal = -1;
        public int cVal = -1;
        public int aVal = -1;

        public int RealNVal => nVal < 0 ? Target.N : nVal;
        public int RealEVal => eVal < 0 ? Target.E : eVal;
        public int RealSVal => sVal < 0 ? Target.S : sVal;
        public int RealWVal => wVal < 0 ? Target.W : wVal;
        public int RealCVal => cVal < 0 ? Target.C : cVal;
        public int RealAVal => aVal < 0 ? Target.A : aVal;

        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null)
                throw new NullCardException(TargetWasNull);
            else if (forbidNotBoard && Target.Location != CardLocation.Field)
                throw new InvalidLocationException(Target.Location, Target, ChangedStatsOfCardOffBoard);

            Target.SetStats((RealNVal, RealEVal, RealSVal, RealWVal, RealCVal, RealAVal), Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}