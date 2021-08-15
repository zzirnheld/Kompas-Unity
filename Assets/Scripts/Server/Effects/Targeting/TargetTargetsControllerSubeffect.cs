using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetTargetsControllerSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));
            Effect.players.Add(Target.Controller);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}