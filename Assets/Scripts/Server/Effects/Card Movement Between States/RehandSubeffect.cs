using KompasCore.Cards.Movement;
using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class RehandSubeffect : CardChangeStateSubeffect
    {
        public override bool IsImpossible() => CardTarget == null;

        public override Task<ResolutionInfo> Resolve()
        {
            if (CardTarget == null) throw new NullCardException(TargetWasNull);

            CardTarget.Rehand(CardTarget.Owner, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}