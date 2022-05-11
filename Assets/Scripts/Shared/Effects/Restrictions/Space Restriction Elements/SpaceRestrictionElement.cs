using KompasCore.Effects.Identities;
using KompasCore.Effects.Relationships;
using KompasServer.Effects.Identities;

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
            public INoActivationContextSpaceIdentity distanceTo;
            public INoActivationContextNumberIdentity number;
            public INumberRelationship comparison;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                distanceTo.Initialize(initializationContext);
                number.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidSpace(Space space, ActivationContext context)
            {
                var origin = this.distanceTo.Space;
                int distance = origin.DistanceTo(space);

                int number = this.number.Number;

                return comparison.Compare(distance, number);
            }
        }

        /// <summary>
        /// Simplifies the adjacency case, even though it could just be done with "compare distance to 1".
        /// </summary>
        public class AdjacentTo : SpaceRestrictionElement
        {
            public INoActivationContextCardIdentity card;
            public INoActivationContextSpaceIdentity space;

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
                if (card != null) return card.Card.IsAdjacentTo(space);
                else if (space != null) return space.AdjacentTo(space);
                else throw new System.NotImplementedException($"You forgot to account for some weird case for {InitializationContext.source}");
            }
        }

        /// <summary>
        /// Whether a card can be moved to that space. Presumes from effect
        /// </summary>
        public class CanMoveCard : SpaceRestrictionElement
        {
            public INoActivationContextCardIdentity toMove;

            public bool normalMove = false;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                toMove.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidSpace(Space space, ActivationContext context)
                => normalMove 
                ? toMove.Card.Card.MovementRestriction.IsValidNormalMove(space)
                : toMove.Card.Card.MovementRestriction.IsValidEffectMove(space, context);
        }
    }
}