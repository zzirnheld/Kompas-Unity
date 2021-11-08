using KompasCore.Effects;
using KompasCore.Exceptions;
using System.Linq;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetAugmentsSubeffect : ServerSubeffect
    {
        public CardRestriction cardRestriction;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction ??= new CardRestriction();
            cardRestriction.Initialize(this);
        }

        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) throw new NullCardException(TargetWasNull);
            else if (!Target.AugmentsList.Any(c => cardRestriction.Evaluate(c, Context)))
                return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

            var potentialTargets = Target.AugmentsList.Where(c => cardRestriction.Evaluate(c, Context));
            foreach(var c in potentialTargets) ServerEffect.AddTarget(c);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}