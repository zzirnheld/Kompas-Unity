using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetThisSpaceSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            ServerEffect.coords.Add((ThisCard.BoardX, ThisCard.BoardY));
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}