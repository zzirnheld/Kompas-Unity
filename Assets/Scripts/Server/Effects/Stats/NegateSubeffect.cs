using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class NegateSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            CardTarget.SetNegated(true, ServerEffect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}