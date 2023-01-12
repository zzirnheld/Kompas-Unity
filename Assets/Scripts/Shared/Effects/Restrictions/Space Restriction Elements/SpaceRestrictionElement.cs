using KompasCore.Cards;
using KompasCore.Effects.Identities;
using KompasCore.Effects.Relationships;
using KompasServer.Effects.Identities;
using KompasCore.Effects.Identities.GamestateNumberIdentities;
using System.Collections.Generic;
using System.Linq;

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
        public class Not : SpaceRestrictionElement
        {
            public SpaceRestrictionElement negated;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                negated.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidSpace(Space space, ActivationContext context)
                => !negated.IsValidSpace(space, context);
        }

        public class AnyOf : SpaceRestrictionElement
        {
            public SpaceRestrictionElement[] restrictions;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                foreach (var r in restrictions) r.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidSpace(Space space, ActivationContext context)
                => restrictions.Any(r => r.IsValidSpace(space, context));
        }

        public class Empty : SpaceRestrictionElement
        {
            protected override bool AbstractIsValidSpace(Space space, ActivationContext context)
                => InitializationContext.game.BoardController.IsEmpty(space);
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
            public INoActivationContextIdentity<IReadOnlyCollection<GameCardBase>> anyOfTheseCards;
            public INoActivationContextIdentity<GameCardBase> card;
            public INoActivationContextIdentity<Space> space;

            private int CountNonNull(params object[] objs) => objs.Where(o => o != null).Count();

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                anyOfTheseCards?.Initialize(initializationContext);
                card?.Initialize(initializationContext);
                space?.Initialize(initializationContext);
                if (card == null && space == null && anyOfTheseCards == null)
                    throw new System.NotImplementedException($"Forgot to provide a space or card to be adjacent to " +
                        $"in the effect of {InitializationContext.source}");
                else if (CountNonNull(card, space, anyOfTheseCards) > 1)
                    throw new System.NotImplementedException($"Provided both a space and a card to be adjacent to " +
                        $"in the effect of {InitializationContext.source}");
            }

            protected override bool AbstractIsValidSpace(Space toTest, ActivationContext context)
            {
                if (anyOfTheseCards != null) return anyOfTheseCards.Item.Any(c => c.IsAdjacentTo(toTest));
                else if (card != null) return card.Item.IsAdjacentTo(toTest);
                else if (space != null) return space.Item.AdjacentTo(toTest);
                else throw new System.NotImplementedException($"You forgot to account for some weird case for {InitializationContext.source}");
            }
        }

        public class WithinDistanceOfNumberOfCards : SpaceRestrictionElement
        {
            public CardRestriction cardRestriction;

            public INoActivationContextIdentity<int> numberOfCards = Constant.ONE;
            public INoActivationContextIdentity<int> distance = Constant.ONE;

            public bool excludeSelf = true;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                cardRestriction.Initialize(initializationContext);
                numberOfCards.Initialize(initializationContext);
                distance.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidSpace(Space space, ActivationContext context)
            {
                return InitializationContext.game.Cards
                    .Where(c => c.DistanceTo(space) < distance.Item)
                    .Where(c => cardRestriction.IsValidCard(c, context))
                    .Count() >= numberOfCards.Item;
            }
        }

        /// <summary>
        /// Whether a card can be moved to that space. Presumes from effect
        /// </summary>
        public class CanMoveCard : SpaceRestrictionElement
        {
            public INoActivationContextIdentity<GameCardBase> toMove;

            /// <summary>
            /// Describes any restriction on the spaces between the card and where it needs to go (the space being tested)
            /// </summary>
            public SpaceRestriction throughRestriction = new SpaceRestriction() {
                spaceRestrictionElements = new List<SpaceRestrictionElement> {
                    new Empty()
                }
            };

            public NumberRestriction distanceRestriction = new NumberRestriction() {
                numberRestrictions = { }
            };

            public bool normalMove = false;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                toMove.Initialize(initializationContext);
            }

            private bool FitsMovementRestriction(GameCardBase card, Space space, ActivationContext context)
                => normalMove 
                    ? card.MovementRestriction.IsValidNormalMove(space)
                    : card.MovementRestriction.IsValidEffectMove(space, context);

            private bool FitsThroughRestriction(Space source, Space dest, ActivationContext context)
                => InitializationContext.game.BoardController.AreConnectedByNumberOfSpacesFittingPredicate(source, dest,
                    s => throughRestriction.IsValidSpace(s, context), distanceRestriction.IsValidNumber);

            protected override bool AbstractIsValidSpace(Space space, ActivationContext context)
            {
                var card = toMove.Item.Card;
                return FitsMovementRestriction(card, space, context) && FitsThroughRestriction(card.Position, space, context);
            }

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
                var card = InitializationContext.game.BoardController.GetCardAt(space);
                return restriction.IsValidCard(card, context);
            }
        }

        public class ConnectedTo : SpaceRestrictionElement
        {
            public INoActivationContextIdentity<IReadOnlyCollection<Space>> spaces;
            public INoActivationContextIdentity<IReadOnlyCollection<Space>> anyOfTheseSpaces;
            public SpaceRestriction byRestriction;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                if (spaces == null && anyOfTheseSpaces == null) throw new System.ArgumentNullException("spaces", "Failed to provide spaces for space restriction elements");

                spaces?.Initialize(initializationContext);
                anyOfTheseSpaces?.Initialize(initializationContext);
                byRestriction.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidSpace(Space space, ActivationContext context)
            {
                return spaces.Item.All(s => InitializationContext.game.BoardController.AreConnectedBySpaces(s, space, byRestriction, context));
            }
        }

        public class InAOEOf : SpaceRestrictionElement
        {
            public INoActivationContextIdentity<GameCardBase> card;
            public INoActivationContextIdentity<IReadOnlyCollection<GameCardBase>> anyOf;
            public INoActivationContextIdentity<IReadOnlyCollection<GameCardBase>> allOf;

            public INoActivationContextIdentity<int> minAnyOfCount;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                card?.Initialize(initializationContext);
                anyOf?.Initialize(initializationContext);
                allOf?.Initialize(initializationContext);

                if (new object[] { card, anyOf, allOf }.All(o => o == null))
                    throw new System.ArgumentNullException("card", $"Provided no card/s to be in AOE of for {initializationContext.source?.CardName}");

                if (minAnyOfCount == null) minAnyOfCount = Constant.ONE;
                minAnyOfCount.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidSpace(Space space, ActivationContext context)
            {
                if (card != null && !card.Item.SpaceInAOE(space)) return false;
                if (anyOf != null && anyOf.Item.Count(c => c.SpaceInAOE(space)) < minAnyOfCount.Item) return false;
                if (allOf != null && !allOf.Item.All(c => c.SpaceInAOE(space))) return false;

                return true;
            }
        }

        public class SameDiagonal : SpaceRestrictionElement
        {
            public INoActivationContextIdentity<Space> space;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                space.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidSpace(Space space, ActivationContext context)
                => space.SameDiagonal(this.space.Item);
        }
    }
}