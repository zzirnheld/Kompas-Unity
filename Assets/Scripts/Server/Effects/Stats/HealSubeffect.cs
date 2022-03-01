using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class HealSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (CardTarget == null)
                throw new NullCardException(TargetWasNull);
            else if (forbidNotBoard && CardTarget.Location != CardLocation.Board)
                throw new InvalidLocationException(CardTarget.Location, CardTarget, ChangedStatsOfCardOffBoard);
            else if (CardTarget.E >= CardTarget.BaseE)
                throw new InvalidCardException(CardTarget, TooMuchEForHeal);

            CardTarget.SetE(CardTarget.BaseE, stackSrc: ServerEffect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}