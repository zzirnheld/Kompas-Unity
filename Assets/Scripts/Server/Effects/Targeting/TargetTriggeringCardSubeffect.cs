using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetTriggeringCardSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (ServerEffect.CurrActivationContext.CardInfo == null)
                return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));

            ServerEffect.AddTarget(ServerEffect.CurrActivationContext.CardInfo.Card);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}