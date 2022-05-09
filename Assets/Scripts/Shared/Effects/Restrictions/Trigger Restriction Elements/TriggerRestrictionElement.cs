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
            public ActivationContextCardIdentity firstCardIdentity;
            public ActivationContextCardIdentity secondCardIdentity;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                firstCardIdentity.Initialize(RestrictionContext);
                secondCardIdentity.Initialize(RestrictionContext);
            }

            protected override bool AbstractIsValidContext(ActivationContext context)
            {
                var first = firstCardIdentity.CardFrom(context);
                var second = secondCardIdentity.CardFrom(context);
                return first.Card == second.Card;
            }
        }


        /// <summary>
        /// An element of 
        /// </summary>
        public class SpaceFitsRestriction : TriggerRestrictionElement
        {
            public SpaceRestriction spaceRestriction;
            public ActivationContextSpaceIdentity spaceIdentity;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                spaceIdentity.Initialize(RestrictionContext);
            }

            protected override bool AbstractIsValidContext(ActivationContext context)
            {
                Space space = spaceIdentity.SpaceFrom(context);
                return spaceRestriction.IsValidSpace(space, context);
            }
        }

        public class CardFitsRestriction : TriggerRestrictionElement
        {
            public CardRestriction cardRestriction;
            public ActivationContextCardIdentity activationContextCardIdentity;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                activationContextCardIdentity.Initialize(RestrictionContext);
            }

            protected override bool AbstractIsValidContext(ActivationContext context)
            {
                var card = activationContextCardIdentity.CardFrom(context);
                return cardRestriction.IsValidCard(card, context);
            }
        }
    }
}