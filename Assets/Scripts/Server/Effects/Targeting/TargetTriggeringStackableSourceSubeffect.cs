using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetTriggeringStackableSourceSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (ServerEffect.CurrActivationContext.Stackable == null)
                return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));

            ServerEffect.AddTarget(ServerEffect.CurrActivationContext.Stackable.Source);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}