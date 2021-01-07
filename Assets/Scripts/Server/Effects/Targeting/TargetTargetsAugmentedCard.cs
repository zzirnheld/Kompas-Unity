using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetTargetsAugmentedCardSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null || Target.AugmentedCard == null) 
                return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));
            else
            {
                ServerEffect.AddTarget(Target.AugmentedCard);
                return Task.FromResult(ResolutionInfo.Next);
            }
        }
    }
}