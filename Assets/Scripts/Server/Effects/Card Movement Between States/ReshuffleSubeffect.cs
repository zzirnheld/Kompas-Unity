using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class ReshuffleSubeffect : CardChangeStateSubeffect
    {
        public override bool IsImpossible() => Target == null;

        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));

            Target.Reshuffle(Target.Owner, Effect); 
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}