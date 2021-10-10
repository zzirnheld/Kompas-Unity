using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetThisSpaceSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            ServerEffect.AddSpace(ThisCard.Position);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}