using KompasCore.Cards;
using KompasCore.Effects.Identities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Restrictions.elements
{
    public class InAOEOf : SpaceRestrictionElement
    {
        public IIdentity<GameCardBase> card;
        public CardRestriction cardRestriction;
        public IIdentity<IReadOnlyCollection<GameCardBase>> anyOf;
        public IIdentity<IReadOnlyCollection<GameCardBase>> allOf;

        public IIdentity<int> minAnyOfCount = Identities.Numbers.Constant.One;

        public IIdentity<Space> alsoInAOE;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            card?.Initialize(initializationContext);
            cardRestriction?.Initialize(initializationContext);
            anyOf?.Initialize(initializationContext);
            allOf?.Initialize(initializationContext);

            if (AllNull(card, cardRestriction, anyOf, allOf))
                throw new System.ArgumentNullException("card", $"Provided no card/s to be in AOE of for {initializationContext.source?.CardName}");

            minAnyOfCount.Initialize(initializationContext);

            alsoInAOE?.Initialize(initializationContext);
        }

        protected override bool IsValidLogic(Space space, IResolutionContext context)
        {
            var isValidCard = IsValidAOE(space, context);
            if (card != null && !ValidateCard(isValidCard, context)) return false;
            if (anyOf != null && !ValidateAnyOf(isValidCard, context)) return false;
            if (allOf != null && !ValidateAllOf(isValidCard, context)) return false;
            return true;
        }

        private Func<GameCardBase, bool> IsValidAOE(Space space, IResolutionContext context)
        {
            var alsoInAOE = this.alsoInAOE?.From(context, default);
            if (alsoInAOE == null) return card => card.SpaceInAOE(space);
            else return card => card.SpaceInAOE(space) && card.SpaceInAOE(alsoInAOE);
        }

        private bool ValidateCard(Func<GameCardBase, bool> IsValidCard, IResolutionContext context)
            => IsValidCard(card.From(context, default));

        private bool ValidateAnyOf(Func<GameCardBase, bool> IsValidCard, IResolutionContext context) 
            => minAnyOfCount.From(context, default)
                <= anyOf.From(context, default).Count(IsValidCard);

        private bool ValidateAllOf(Func<GameCardBase, bool> IsValidCard, IResolutionContext context)
            => allOf.From(context, default).All(IsValidCard);
    }
}