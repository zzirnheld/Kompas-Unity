using KompasCore.Cards;
using KompasCore.Effects.Identities;
using KompasCore.Effects.Identities.GamestateNumberIdentities;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Restrictions.SpaceRestrictionElements
{
    public class InAOEOf : SpaceRestrictionElement
    {
        public INoActivationContextIdentity<GameCardBase> card;
        public INoActivationContextIdentity<IReadOnlyCollection<GameCardBase>> anyOf;
        public INoActivationContextIdentity<IReadOnlyCollection<GameCardBase>> allOf;

        public INoActivationContextIdentity<int> minAnyOfCount;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            card?.Initialize(initializationContext);
            anyOf?.Initialize(initializationContext);
            allOf?.Initialize(initializationContext);

            if (new object[] { card, anyOf, allOf }.All(o => o == null))
                throw new System.ArgumentNullException("card", $"Provided no card/s to be in AOE of for {initializationContext.source?.CardName}");

            if (minAnyOfCount == null) minAnyOfCount = Constant.ONE;
            minAnyOfCount.Initialize(initializationContext);
        }

        protected override bool AbstractIsValidSpace(Space space, ActivationContext context)
        {
            if (card != null && !card.Item.SpaceInAOE(space)) return false;
            if (anyOf != null && anyOf.Item.Count(c => c.SpaceInAOE(space)) < minAnyOfCount.Item) return false;
            if (allOf != null && !allOf.Item.All(c => c.SpaceInAOE(space))) return false;

            return true;
        }
    }
}