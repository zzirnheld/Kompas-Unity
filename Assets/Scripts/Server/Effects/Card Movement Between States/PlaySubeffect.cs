using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class PlaySubeffect : CardChangeStateSubeffect
    {
        public override bool IsImpossible() => Target == null || Target.Location == CardLocation.Field;

        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));

            Target.Play(Space, Player, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}