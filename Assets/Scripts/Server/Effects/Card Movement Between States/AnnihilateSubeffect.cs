using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class AnnihilateSubeffect : CardChangeStateSubeffect
    {
        public override bool IsImpossible() => Target == null || Target.Location == CardLocation.Annihilation;

        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));
            else if (Target.Location == CardLocation.Annihilation) return Task.FromResult(ResolutionInfo.Impossible(TargetAlreadyThere));

            Target.Owner.annihilationCtrl.Annihilate(Target, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}