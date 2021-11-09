using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class AnnihilateSubeffect : CardChangeStateSubeffect
    {
        public override bool IsImpossible() => Target == null || Target.Location == CardLocation.Annihilation;

        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) throw new NullCardException(TargetWasNull);

            Target.Owner.annihilationCtrl.Annihilate(Target, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}