using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class DiscardSubeffect : CardChangeStateSubeffect
    {
        public override bool IsImpossible() => Target == null;

        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));

            Target.Discard(ServerEffect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}