using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TopdeckSubeffect : CardChangeStateSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));
            else if (Target.Topdeck(Target.Owner, Effect)) return Task.FromResult(ResolutionInfo.Next);
            else return Task.FromResult(ResolutionInfo.Impossible(TopdeckFailed));
        }
    }
}