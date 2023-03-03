using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffect
{
    public class TargetTargetsControllerSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (CardTarget == null) throw new NullCardException(TargetWasNull);
            Effect.playerTargets.Add(CardTarget.Controller);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}