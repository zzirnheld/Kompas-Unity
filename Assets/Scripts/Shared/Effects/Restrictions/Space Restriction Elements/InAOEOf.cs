using KompasCore.Cards;
using KompasCore.Effects.Identities;
using KompasCore.Effects.Identities.Numbers;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Restrictions.SpaceRestrictionElements
{
    public class InAOEOf : SpaceRestrictionElement
    {
        public IIdentity<GameCardBase> card;
        public IIdentity<IReadOnlyCollection<GameCardBase>> anyOf;
        public IIdentity<IReadOnlyCollection<GameCardBase>> allOf;

        public IIdentity<int> minAnyOfCount;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            card?.Initialize(initializationContext);
            anyOf?.Initialize(initializationContext);
            allOf?.Initialize(initializationContext);

            if (new object[] { card, anyOf, allOf }.All(o => o == null))
                throw new System.ArgumentNullException("card", $"Provided no card/s to be in AOE of for {initializationContext.source?.CardName}");

            if (minAnyOfCount == null) minAnyOfCount = Constant.One;
            minAnyOfCount.Initialize(initializationContext);
        }

        protected override bool AbstractIsValidSpace(Space space, ActivationContext context)
        {
            if (card != null && !ValidateCard(space, context)) return false;

            if (anyOf != null && !ValidateAnyOf(space, context)) return false;

            if (allOf != null && !ValidateAllOf(space, context)) return false;

            return true;
        }

        private bool ValidateCard(Space space, ActivationContext context) => card.From(context, default).SpaceInAOE(space);

        private bool ValidateAnyOf(Space space, ActivationContext context) 
            => minAnyOfCount.From(context, default) <= anyOf.From(context, default)
                                                            .Count(c => c.SpaceInAOE(space));

        private bool ValidateAllOf(Space space, ActivationContext context)
            => allOf.From(context, default).All(c => c.SpaceInAOE(space));
    }
}