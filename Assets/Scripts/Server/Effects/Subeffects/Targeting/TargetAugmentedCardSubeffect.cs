using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetAugmentedCardSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (!ServerEffect.Source.Attached) throw new NullCardException(NoValidCardTarget);
            else
            {
                ServerEffect.AddTarget(Source.AugmentedCard);
                return Task.FromResult(ResolutionInfo.Next);
            }
        }
    }
}