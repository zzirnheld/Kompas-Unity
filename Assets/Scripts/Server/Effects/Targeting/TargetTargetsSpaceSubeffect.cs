using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetTargetsSpaceSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            Effect.coords.Add(Target.Position);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}