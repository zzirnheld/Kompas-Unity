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
            if (CardTarget == null) throw new NullCardException(TargetWasNull);
            else if (!CardTarget.Augments.Any(c => cardRestriction.IsValidCard(c, CurrentContext)))
                return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

            var potentialTargets = CardTarget.Augments.Where(c => cardRestriction.IsValidCard(c, CurrentContext));
            foreach (var c in potentialTargets) ServerEffect.AddTarget(c);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}