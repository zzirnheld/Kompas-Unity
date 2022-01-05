using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetTargetsAugmentedCardSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) throw new NullCardException(NoValidCardTarget);
            else if (!Target.Attached) throw new NullCardException(NoValidCardTarget);
            else
            {
                ServerEffect.AddTarget(Target.AugmentedCard);
                return Task.FromResult(ResolutionInfo.Next);
            }
        }
    }
}