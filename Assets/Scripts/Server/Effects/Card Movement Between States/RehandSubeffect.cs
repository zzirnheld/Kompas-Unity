using System.Threading.Tasks;

namespace KompasServer.Effects
{
    [System.Serializable]
    public class RehandSubeffect : CardChangeStateSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));
            else if (Target.Rehand(Target.Owner, Effect)) return Task.FromResult(ResolutionInfo.Next);
            else return Task.FromResult(ResolutionInfo.Impossible(RehandFailed));
        }
    }
}