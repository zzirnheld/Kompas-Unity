using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetAvatarSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            Effect.AddTarget(Player.Avatar);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}