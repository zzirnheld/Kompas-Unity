using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetTriggeringCoordsSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (ServerEffect.CurrActivationContext.Space == null)
                return Task.FromResult(ResolutionInfo.Impossible(NoValidSpaceTarget));

            ServerEffect.coords.Add(ServerEffect.CurrActivationContext.Space.Value);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}