using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class TargetThisSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            ServerEffect.AddTarget(ThisCard);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}