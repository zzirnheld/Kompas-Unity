using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class EndResolution : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            return Task.FromResult(ResolutionInfo.End(EndOnPurpose));
        }
    }
}