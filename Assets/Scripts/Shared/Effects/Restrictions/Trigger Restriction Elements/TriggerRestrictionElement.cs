using KompasCore.Effects.Identities;
using System;

namespace KompasCore.Effects.Restrictions
{
    public abstract class TriggerRestrictionElement : ContextInitializeableBase, IContextInitializeable
    {
        public bool primaryContext = true;

        public bool IsValidContext(ActivationContext context, ActivationContext secondaryContext = default)
        {
            ComplainIfNotInitialized();

            ActivationContext contextToConsider = primaryContext ? context : secondaryContext;
            return AbstractIsValidContext(contextToConsider);
        }

        protected abstract bool AbstractIsValidContext(ActivationContext context);
    }

    namespace TriggerRestrictionElements
    {
        public class ThisCardInPlay : TriggerRestrictionElement
        {
            protected override bool AbstractIsValidContext(ActivationContext context)
                => RestrictionContext.source.Location == CardLocation.Board;
        }

        public class CardsMatch : TriggerRestrictionElement
        {
            public IActivationContextCardIdentity firstCard;
            public IActivationContextCardIdentity secondCard;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                firstCard.Initialize(RestrictionContext);
                secondCard.Initialize(RestrictionContext);
            }

            protected override bool AbstractIsValidContext(ActivationContext context)
            {
                var first = firstCard.CardFrom(context);
                var second = secondCard.CardFrom(context);
                return first.Card == second.Card;
            }
        }


        /// <summary>
        /// An element of 
        /// </summary>
        public class SpaceFitsRestriction : TriggerRestrictionElement
        {
            public SpaceRestriction spaceRestriction;
            public IActivationContextSpaceIdentity space;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                space.Initialize(RestrictionContext);
                spaceRestriction.Initialize(restrictionContext);
            }

            protected override bool AbstractIsValidContext(ActivationContext context)
                => spaceRestriction.IsValidSpace(space.SpaceFrom(context), context);
        }

        public class CardFitsRestriction : TriggerRestrictionElement
        {
            public CardRestriction cardRestriction;
            public IActivationContextCardIdentity card;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                card.Initialize(restrictionContext);
                cardRestriction.Initialize(restrictionContext);
            }

            protected override bool AbstractIsValidContext(ActivationContext context)
            {
                var card = this.card.CardFrom(context);
                return cardRestriction.IsValidCard(card, context);
            }
        }
    }
}