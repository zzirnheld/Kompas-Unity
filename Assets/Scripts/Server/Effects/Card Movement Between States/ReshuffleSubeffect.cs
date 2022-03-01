using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class ReshuffleSubeffect : CardChangeStateSubeffect
    {
        public override bool IsImpossible() => CardTarget == null;

        public override Task<ResolutionInfo> Resolve()
        {
            if (CardTarget == null) throw new NullCardException(TargetWasNull);

            CardTarget.Reshuffle(CardTarget.Owner, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}