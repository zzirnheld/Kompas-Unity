using KompasCore.Cards;
using KompasCore.Effects.Identities;

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
                => InitializationContext.source.Location == CardLocation.Board;
        }

        public class CardsMatch : TriggerRestrictionElement
        {
            public IActivationContextIdentity<GameCardBase> firstCard;
            public IActivationContextIdentity<GameCardBase> secondCard;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                firstCard.Initialize(initializationContext);
                secondCard.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidContext(ActivationContext context)
            {
                var first = firstCard.From(context);
                var second = secondCard.From(context);
                return first.Card == second.Card;
            }
        }


        /// <summary>
        /// An element of 
        /// </summary>
        public class SpaceFitsRestriction : TriggerRestrictionElement
        {
            public SpaceRestriction spaceRestriction;
            public IActivationContextIdentity<Space> space;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                space.Initialize(initializationContext);
                spaceRestriction.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidContext(ActivationContext context)
                => spaceRestriction.IsValidSpace(space.From(context), context);
        }

        public class CardFitsRestriction : TriggerRestrictionElement
        {
            public CardRestriction cardRestriction;
            public IActivationContextIdentity<GameCardBase> card;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                card.Initialize(initializationContext);
                cardRestriction.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidContext(ActivationContext context)
            {
                var card = this.card.From(context);
                return cardRestriction.IsValidCard(card, context);
            }
        }

        public class FriendlyTurn : TriggerRestrictionElement
        {
            protected override bool AbstractIsValidContext(ActivationContext context)
                => InitializationContext.game.TurnPlayer == InitializationContext.source.Controller;
        }

        public class EnemyTurn : TriggerRestrictionElement
        {
            protected override bool AbstractIsValidContext(ActivationContext context)
                => InitializationContext.game.TurnPlayer != InitializationContext.source.Controller;
        }
    }
}