using KompasCore.Cards;
using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions
{
    public abstract class TriggerRestrictionElement : ContextInitializeableBase, IContextInitializeable
    {
        public bool IsValidContext(ActivationContext context, ActivationContext secondaryContext = default)
        {
            ComplainIfNotInitialized();
            return AbstractIsValidContext(context, secondaryContext);
        }

        protected abstract bool AbstractIsValidContext(ActivationContext context, ActivationContext secondaryContext);
    }

    namespace TriggerRestrictionElements
    {
        public class ThisCardInPlay : TriggerRestrictionElement
        {
            protected override bool AbstractIsValidContext(ActivationContext context, ActivationContext secondaryContext)
                => InitializationContext.source.Location == CardLocation.Board;
        }

        public class FriendlyTurn : TriggerRestrictionElement
        {
            protected override bool AbstractIsValidContext(ActivationContext context, ActivationContext secondaryContext)
                => InitializationContext.game.TurnPlayer == InitializationContext.source.Controller;
        }

        public class EnemyTurn : TriggerRestrictionElement
        {
            protected override bool AbstractIsValidContext(ActivationContext context, ActivationContext secondaryContext)
                => InitializationContext.game.TurnPlayer != InitializationContext.source.Controller;
        }

        public class StackablesMatch : TriggerRestrictionElement
        {
            public IActivationContextIdentity<IStackable> firstStackable;
            public IActivationContextIdentity<IStackable> secondStackable;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                firstStackable.Initialize(initializationContext);
                secondStackable.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidContext(ActivationContext context, ActivationContext secondaryContext)
                => firstStackable.From(context, secondaryContext) == secondStackable.From(context, secondaryContext);
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

            protected override bool AbstractIsValidContext(ActivationContext context, ActivationContext secondaryContext)
            {
                var first = firstCard.From(context, secondaryContext);
                var second = secondCard.From(context, secondaryContext);
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

            protected override bool AbstractIsValidContext(ActivationContext context, ActivationContext secondaryContext)
                => spaceRestriction.IsValidSpace(space.From(context, secondaryContext), context);
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

            protected override bool AbstractIsValidContext(ActivationContext context, ActivationContext secondaryContext)
            {
                var card = this.card.From(context, secondaryContext);
                return cardRestriction.IsValidCard(card, context);
            }
        }
    }
}