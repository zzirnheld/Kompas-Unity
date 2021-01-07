using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class AnnihilateSubeffect : CardChangeStateSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));
            else if (Target.Location == CardLocation.Annihilation) return Task.FromResult(ResolutionInfo.Impossible(TargetAlreadyThere));
            else if (Game.annihilationCtrl.Annihilate(Target, Effect)) return Task.FromResult(ResolutionInfo.Next);
            else return Task.FromResult(ResolutionInfo.Impossible(AnnihilationFailed));
        }
    }
}