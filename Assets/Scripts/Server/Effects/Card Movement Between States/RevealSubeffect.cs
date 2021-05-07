using System.Threading.Tasks;

namespace KompasServer.Effects
{
    [System.Serializable]
    public class RevealSubeffect : CardChangeStateSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));
            else if (Target.Reveal(Effect)) return Task.FromResult(ResolutionInfo.Next);
            else return Task.FromResult(ResolutionInfo.Impossible(RehandFailed));
        }
    }
}