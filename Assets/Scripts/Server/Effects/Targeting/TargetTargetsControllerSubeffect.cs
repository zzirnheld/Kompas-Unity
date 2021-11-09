using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetTargetsControllerSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) throw new NullCardException(TargetWasNull);
            Effect.players.Add(Target.Controller);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}