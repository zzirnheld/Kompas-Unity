using KompasCore.Cards;
using KompasCore.Effects.Identities;
using KompasCore.Effects.Relationships;

namespace KompasCore.Effects.Restrictions
{
    public abstract class SpaceRestrictionElement : ContextInitializeableBase, IContextInitializeable
    {
        public bool IsValidSpace(Space space, ActivationContext context)
        {
            ComplainIfNotInitialized();
            return AbstractIsValidSpace(space, context);
        }

        protected abstract bool AbstractIsValidSpace(Space space, ActivationContext context);
    }

    namespace SpaceRestrictionElements
    {
        public class Empty : SpaceRestrictionElement
        {
            protected override bool AbstractIsValidSpace(Space space, ActivationContext context)
                => InitializationContext.game.boardCtrl.IsEmpty(space);
        }

        /// <summary>
        /// Gets the distance between the described origin point and the space to be tested,
        /// gets the described number,
        /// and compares the distance to the number with the given comparison.
        /// </summary>
        public class CompareDistance : SpaceRestrictionElement
        {
            public INoActivationContextIdentity<Space> distanceTo;
            public INoActivationContextIdentity<int> number;
            public INumberRelationship comparison;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                distanceTo.Initialize(initializationContext);
                number.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidSpace(Space space, ActivationContext context)
            {
                var origin = this.distanceTo.Item;
                int distance = origin.DistanceTo(space);

                int number = this.number.Item;

                return comparison.Compare(distance, number);
            }
        }

        /// <summary>
        /// Simplifies the adjacency case, even though it could just be done with "compare distance to 1".
        /// </summary>
        public class AdjacentTo : SpaceRestrictionElement
        {
            public INoActivationContextIdentity<GameCardBase> card;
            public INoActivationContextIdentity<Space> space;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                card.Initialize(initializationContext);
                space.Initialize(initializationContext);
                if (card == null && space == null)
                    throw new System.NotImplementedException($"Forgot to provide a space or card to be adjacent to " +
                        $"in the effect of {InitializationContext.source}");
                else if (card != null && space != null)
                    throw new System.NotImplementedException($"Provided both a space and a card to be adjacent to " +
                        $"in the effect of {InitializationContext.source}");
            }

            protected override bool AbstractIsValidSpace(Space space, ActivationContext context)
            {
                if (card != null) return card.Item.IsAdjacentTo(space);
                else if (space != null) return space.AdjacentTo(space);
                else throw new System.NotImplementedException($"You forgot to account for some weird case for {InitializationContext.source}");
            }
        }

        public class InAOE : SpaceRestrictionElement
        {
            public IActivationContextIdentity<GameCardBase> inAOEOf;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                inAOEOf.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidSpace(Space space, ActivationContext context)
                => inAOEOf.From(context, default).IsSpaceInMyAOE(space);
        }

        /// <summary>
        /// Whether a card can be moved to that space. Presumes from effect
        /// </summary>
        public class CanMoveCard : SpaceRestrictionElement
        {
            public INoActivationContextIdentity<GameCardBase> toMove;

            public bool normalMove = false;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                toMove.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidSpace(Space space, ActivationContext context)
                => normalMove 
                ? toMove.Item.Card.MovementRestriction.IsValidNormalMove(space)
                : toMove.Item.Card.MovementRestriction.IsValidEffectMove(space, context);
        }

        /// <summary>
        /// Whether a card can be moved to that space. Presumes from effect
        /// </summary>
        public class CanPlayCard : SpaceRestrictionElement
        {
            public INoActivationContextIdentity<GameCardBase> toPlay;

            public bool normalPlay = false;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                toPlay.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidSpace(Space space, ActivationContext context)
                => normalPlay
                ? toPlay.Item.Card.PlayRestriction.IsValidNormalPlay(space, InitializationContext.Controller)
                : toPlay.Item.Card.PlayRestriction.IsValidEffectPlay(space, InitializationContext.effect, InitializationContext.Controller, context);
        }

        public class CardFitsRestriction : SpaceRestrictionElement
        {
            public CardRestriction restriction;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                restriction.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidSpace(Space space, ActivationContext context)
            {
                var card = InitializationContext.game.boardCtrl.GetCardAt(space);
                return restriction.IsValidCard(card, context);
            }
        }
    }
}