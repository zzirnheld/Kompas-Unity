using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class DiscardSubeffect : CardChangeStateSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));
            if (Target.Discard(ServerEffect)) return Task.FromResult(ResolutionInfo.Next);
            else return Task.FromResult(ResolutionInfo.Impossible(DiscardFailed));
        }
    }
}