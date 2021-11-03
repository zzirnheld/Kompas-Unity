using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class MoveSubeffect : CardChangeStateSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));

            Target.Move(Space, false, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}