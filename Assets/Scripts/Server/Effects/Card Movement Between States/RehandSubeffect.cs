using System.Threading.Tasks;

namespace KompasServer.Effects
{
    [System.Serializable]
    public class RehandSubeffect : CardChangeStateSubeffect
    {
        public override bool IsImpossible() => Target == null;

        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));

            Target.Rehand(Target.Owner, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}