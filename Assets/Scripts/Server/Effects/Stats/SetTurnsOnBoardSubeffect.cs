using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class SetTurnsOnBoardSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (CardTarget == null)
                throw new NullCardException(TargetWasNull);
            else if (forbidNotBoard && CardTarget.Location != CardLocation.Board)
                throw new InvalidLocationException(CardTarget.Location, CardTarget, ChangedStatsOfCardOffBoard);

            CardTarget.TurnsOnBoard = Count;
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}