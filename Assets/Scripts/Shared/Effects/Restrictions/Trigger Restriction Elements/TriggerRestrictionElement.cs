using KompasCore.Cards;
using KompasCore.Effects.Identities;
using KompasCore.Effects.Relationships;

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

        public class CompareNumbers : TriggerRestrictionElement
        {
            public IActivationContextIdentity<int> firstNumber;
            public IActivationContextIdentity<int> secondNumber;
            public INumberRelationship comparison;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                firstNumber.Initialize(initializationContext);
                secondNumber.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidContext(ActivationContext context, ActivationContext secondaryContext)
            {
                int first = firstNumber.From(context, secondaryContext);
                int second = secondNumber.From(context, secondaryContext);
                return comparison.Compare(first, second);
            }
        }

        public class StackableFitsRestriction : TriggerRestrictionElement
        {
            public StackableRestriction restriction;
            public IActivationContextIdentity<IStackable> stackable;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                restriction.Initialize(initializationContext);
                stackable.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidContext(ActivationContext context, ActivationContext secondaryContext)
                => restriction.Evaluate(stackable.From(context, secondaryContext));
        }

        public class NumberFitsRestriction : TriggerRestrictionElement
        {
            public IActivationContextIdentity<int> number;
            public NumberRestriction restriction;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                number.Initialize(initializationContext);
                restriction.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidContext(ActivationContext context, ActivationContext secondaryContext)
                => restriction.IsValidNumber(number.From(context, secondaryContext));
        }

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