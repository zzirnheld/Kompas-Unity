using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class BottomdeckSubeffect : CardChangeStateSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));
            else if (Target.Bottomdeck(Target.Owner, Effect)) return Task.FromResult(ResolutionInfo.Next);
            else return Task.FromResult(ResolutionInfo.Impossible(BottomdeckFailed));
        }
    }
}