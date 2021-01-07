using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class MoveSubeffect : CardChangeStateSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            var (x, y) = Space;
            if (Target == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));
            else if (Target.Move(x, y, false, Effect)) return Task.FromResult(ResolutionInfo.Next);
            else return Task.FromResult(ResolutionInfo.Impossible(MovementFailed));
        }
    }
}