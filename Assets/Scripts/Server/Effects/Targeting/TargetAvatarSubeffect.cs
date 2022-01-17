using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetAvatarSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            Effect.AddTarget(PlayerTarget.Avatar);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}