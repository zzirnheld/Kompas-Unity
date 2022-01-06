using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class AnnihilateSubeffect : CardChangeStateSubeffect
    {
        public override bool IsImpossible() => CardTarget == null || CardTarget.Location == CardLocation.Annihilation;

        public override Task<ResolutionInfo> Resolve()
        {
            if (CardTarget == null) throw new NullCardException(TargetWasNull);

            CardTarget.Owner.annihilationCtrl.Annihilate(CardTarget, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}