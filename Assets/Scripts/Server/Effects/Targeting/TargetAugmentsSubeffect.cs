using KompasCore.Cards;
using KompasCore.Effects;
using System.Linq;
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

        public override bool Resolve()
        {
            if (Target == null || !Target.AugmentsList.Any(c => cardRestriction.Evaluate(c)))
                return ServerEffect.EffectImpossible();

            var potentialTargets = Target.AugmentsList.Where(c => cardRestriction.Evaluate(c));
            foreach(var c in potentialTargets) ServerEffect.AddTarget(c);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}