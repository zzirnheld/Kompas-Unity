using KompasCore.Cards;
using System.Linq;

namespace KompasCore.Effects.Identities.ActivationContextNumberIdentities
{
    public class X : ContextualLeafIdentityBase<int>
    {
        public int multiplier = 1;
        public int modifier = 0;
        public int divisor = 1;

        protected override int AbstractItemFrom(ActivationContext contextToConsider)
            => (contextToConsider.x.GetValueOrDefault() * multiplier / divisor) + modifier;
    }

    public class Distance : ContextualIdentityBase<int>
    {
        public IIdentity<Space> firstSpace;
        public IIdentity<Space> secondSpace;

        public SpaceRestriction throughRestriction;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            firstSpace.Initialize(initializationContext);
            secondSpace.Initialize(initializationContext);

            throughRestriction?.Initialize(initializationContext);
        }

        protected override int AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
        {
            Space first = firstSpace.From(context, secondaryContext);
            Space second = secondSpace.From(context, secondaryContext);


            if (first == null || second == null) return -1;

            if (throughRestriction == null) return first.DistanceTo(second);

            var contextToConsider = secondary ? secondaryContext : context;
            var predicate = throughRestriction.AsThroughPredicate(contextToConsider);
            return InitializationContext.game.BoardController.ShortestPath(first, second, predicate);
        }
    }

    public class Operation : ContextualIdentityBase<int>
    {
        public IIdentity<int>[] numbers;
        public INumberOperation operation;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            foreach(var identity in numbers)
            {
                identity.Initialize(initializationContext);
            }
        }

        protected override int AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
        {
            var numberValues = numbers.Select(n => n.From(context, secondaryContext)).ToArray();
            return operation.Perform(numberValues);
        }
    }

    public class FromCardValue : ContextualIdentityBase<int>
    {
        public IIdentity<GameCardBase> card;
        public CardValue cardValue;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            card.Initialize(initializationContext);
            cardValue.Initialize(initializationContext);
        }

        protected override int AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
            => cardValue.GetValueOf(card.From(context, secondaryContext));
    }

    public class Arg : ContextlessLeafIdentityBase<int>
    {
        protected override int AbstractItem => InitializationContext.subeffect.Effect.arg;
    }

    public class TargetCount : ContextualIdentityBase<int>
    {
        public CardRestriction cardRestriction;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            cardRestriction?.Initialize(initializationContext);
        }

        private System.Func<GameCardBase, bool> Selector(ActivationContext context)
            => card => cardRestriction?.IsValidCard(card, InitializationContext.effect.CurrActivationContext) ?? true;

        protected override int AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
            => InitializationContext.subeffect.Effect.CardTargets.Count(Selector(context));
    }
}