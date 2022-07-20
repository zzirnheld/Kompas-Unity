using KompasCore.Cards;
using KompasCore.Effects.Identities;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Restrictions
{
    public abstract class CardRestrictionElement : ContextInitializeableBase
    {
        public bool FitsRestriction(GameCardBase card, ActivationContext context)
        {
            ComplainIfNotInitialized();

            return card != null && FitsRestrictionLogic(card, context);
        }

        protected abstract bool FitsRestrictionLogic(GameCardBase card, ActivationContext context);
    }

    namespace CardRestrictionElements
    {
        public class CardExists : CardRestrictionElement
        {
            protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
                => card != null;
        }

        public class FriendlyCard : CardRestrictionElement
        {
            protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
                => card.Controller == InitializationContext.source.Controller;
        }

        public class EnemyCard : CardRestrictionElement
        {
            protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
                => card.Controller != InitializationContext.source.Controller;
        }

        public class Character : CardRestrictionElement
        {
            protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
                => card.CardType == 'C';
        }

        public class Location : CardRestrictionElement
        {
            public string[] locations;

            private ICollection<CardLocation> Locations => locations.Select(CardLocationHelpers.FromString).ToArray();

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                if (locations == null) throw new System.ArgumentNullException("locations");
            }

            protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
                => Locations.Any(loc => card.Location == loc);
        }

        public class PositionFitsRestriction : CardRestrictionElement
        {
            public SpaceRestriction spaceRestriction;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                spaceRestriction.Initialize(initializationContext);
            }

            protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
                => spaceRestriction.IsValidSpace(card.Position, context);
        }

        /// <summary>
        /// Helper class for checking whether a card is adjacent to another card.
        /// For simplicity reasons, default checks whether the card is adjacent to this one.
        /// </summary>
        public class AdjacentTo : CardRestrictionElement
        {
            public INoActivationContextIdentity<GameCardBase> adjacentTo = new Identities.GamestateCardIdentities.ThisCard();

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                adjacentTo.Initialize(initializationContext);
            }

            protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
                => adjacentTo.Item.IsAdjacentTo(card);
        }

        public class SubtypesInclude : CardRestrictionElement
        {
            public string[] subtypes;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                if (subtypes == null) throw new System.ArgumentNullException("subtypes");
            }

            protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
                => subtypes.All(subtype => card.SubtypeText.Contains(subtype));
        }

        public class CanMoveTo : CardRestrictionElement
        {
            //public IActivationContextIdentity<Space> contextDestination;
            public INoActivationContextIdentity<Space> destination;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                /*if (contextDestination == null && noContextDestiantion == null)
                    throw new System.ArgumentNullException("CanMoveTo has neither a contextual, nor a non-contextual, destination");

                contextDestination?.Initialize(initializationContext);*/
                destination?.Initialize(initializationContext);
            }

            protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
                => card.MovementRestriction.IsValidEffectMove(destination.Item, context);
        }

        public class Not : CardRestrictionElement
        {
            public CardRestrictionElement element;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                element.Initialize(initializationContext);
            }

            protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
                => !element.FitsRestriction(card, context);
        }

        public class Fighting : CardRestrictionElement
        {
            /// <summary>
            /// Can be null to represent checking whether the card is in any fight at all
            /// </summary>
            public IActivationContextIdentity<GameCardBase> fightingWho;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                fightingWho?.Initialize(initializationContext);
            }

            private bool IsValidFight(GameCardBase card, ActivationContext context, IStackable stackEntry)
            {
                var fightingWhoCard = fightingWho?.From(context, default);
                return stackEntry is Attack attack
                    && (attack.attacker == card || attack.defender == card)
                    && (fightingWho == null || attack.attacker == fightingWhoCard || attack.defender == fightingWhoCard);
            }

            protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
                => InitializationContext.game.StackEntries.Any(stackEntry => IsValidFight(card, context, stackEntry));
        }
    }
}