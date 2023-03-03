using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
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