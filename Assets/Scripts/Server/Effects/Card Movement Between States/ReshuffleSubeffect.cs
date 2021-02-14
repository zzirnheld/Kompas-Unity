using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class ReshuffleSubeffect : CardChangeStateSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));
            else if (Target.Reshuffle(Target.Owner, Effect)) return Task.FromResult(ResolutionInfo.Next);
            else return Task.FromResult(ResolutionInfo.Impossible(ReshuffleFailed));
        }
    }
}