using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class AnnihilateSubeffect : CardChangeStateSubeffect
    {
        public override async Task<ResolutionInfo> Resolve()
        {
            if (Target == null) return ResolutionInfo.Impossible(TargetWasNull);
            else if (Target.Location == CardLocation.Annihilation) return ResolutionInfo.Impossible(TargetAlreadyThere);
            else if (Game.annihilationCtrl.Annihilate(Target, Effect)) return ResolutionInfo.Next;
            else return ResolutionInfo.Impossible(AnnihilationFailed);
        }
    }
}