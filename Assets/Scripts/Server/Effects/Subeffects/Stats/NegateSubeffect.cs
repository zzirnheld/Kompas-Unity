using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class NegateSubeffect : ServerSubeffect
    {
        public bool negated = true;

        public override Task<ResolutionInfo> Resolve()
        {
            CardTarget.SetNegated(negated, ServerEffect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}