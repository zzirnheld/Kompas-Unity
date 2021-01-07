using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class EndResolutionSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            return Task.FromResult(ResolutionInfo.End(EndOnPurpose));
        }
    }
}