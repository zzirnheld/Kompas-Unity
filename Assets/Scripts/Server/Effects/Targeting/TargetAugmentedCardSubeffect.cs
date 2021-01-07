using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetAugmentedCardSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (ServerEffect.Source.AugmentedCard == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));
            else
            {
                ServerEffect.AddTarget(Source.AugmentedCard);
                return Task.FromResult(ResolutionInfo.Next);
            }
        }
    }
}