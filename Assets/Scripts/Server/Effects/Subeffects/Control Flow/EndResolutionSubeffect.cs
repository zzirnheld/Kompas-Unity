using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffect
{
    public class EndResolutionSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            return Task.FromResult(ResolutionInfo.End(EndOnPurpose));
        }
    }
}