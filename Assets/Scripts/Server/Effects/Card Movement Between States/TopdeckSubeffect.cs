using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TopdeckSubeffect : CardChangeStateSubeffect
    {
        public override bool IsImpossible() => Target == null;

        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));

            Target.Topdeck(Target.Owner, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}