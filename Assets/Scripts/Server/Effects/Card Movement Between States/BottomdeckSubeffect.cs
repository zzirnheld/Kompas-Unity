using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class BottomdeckSubeffect : CardChangeStateSubeffect
    {
        public override async Task<ResolutionInfo> Resolve()
        {
            if (Target == null) return ResolutionInfo.Impossible(TargetWasNull);
            else if (Target.Bottomdeck(Target.Owner, Effect)) return ResolutionInfo.Next;
            else return ResolutionInfo.Impossible(BottomdeckFailed);
        }
    }
}