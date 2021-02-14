using KompasCore.Cards;
using KompasCore.Effects;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class TargetAugmentsSubeffect : ServerSubeffect
    {
        public CardRestriction cardRestriction;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction = cardRestriction ?? new CardRestriction();
            cardRestriction.Initialize(this);
        }

        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));
            if (!Target.AugmentsList.Any(c => cardRestriction.Evaluate(c)))
                return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

            var potentialTargets = Target.AugmentsList.Where(c => cardRestriction.Evaluate(c));
            foreach(var c in potentialTargets) ServerEffect.AddTarget(c);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}