using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class PlaySubeffect : CardChangeStateSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            var (x, y) = Space;
            if (Target == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));
            else if (Target.Play(x, y, Player, Effect)) return Task.FromResult(ResolutionInfo.Next);
            else return Task.FromResult(ResolutionInfo.Impossible(PlayingFailed));
        }
    }
}