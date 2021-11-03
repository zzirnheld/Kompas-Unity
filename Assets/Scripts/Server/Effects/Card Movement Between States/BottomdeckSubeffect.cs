using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class BottomdeckSubeffect : CardChangeStateSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));

            Target.Bottomdeck(Target.Owner, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}