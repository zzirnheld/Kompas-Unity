using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class ThisSpaceTargetSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            ServerEffect.coords.Add(ThisCard.Position);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}