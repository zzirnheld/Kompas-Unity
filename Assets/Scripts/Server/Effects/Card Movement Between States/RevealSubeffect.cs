using System.Threading.Tasks;

namespace KompasServer.Effects
{
    [System.Serializable]
    public class RevealSubeffect : CardChangeStateSubeffect
    {
        public override bool IsImpossible() => Target == null;

        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));

            Target.Reveal(Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}