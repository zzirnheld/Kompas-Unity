using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetTriggeringCardsCoordsSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (ServerEffect.CurrActivationContext.CardInfo == null)
                return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));

            ServerEffect.coords.Add(ServerEffect.CurrActivationContext.CardInfo.Position);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}