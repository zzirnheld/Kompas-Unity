using KompasCore.Cards;
using KompasCore.Effects.Identities;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Restrictions
{
    public abstract class CardRestrictionElement : ContextInitializeableBase
    {
        public bool FitsRestriction(GameCardBase card, ActivationContext context) => Initialized ? FitsRestrictionLogic(card, context)
            : throw new System.NotImplementedException("You failed to initialize a Card Restriction Element");

        protected abstract bool FitsRestrictionLogic(GameCardBase card, ActivationContext context);
    }

    namespace CardRestrictionElements
    {
        public class CardExists : CardRestrictionElement
        {
            protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
                => card != null;
        }

        public class EnemyCard : CardRestrictionElement
        {
            protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
                => card.Controller != RestrictionContext.source.Controller;
        }

        public class Location : CardRestrictionElement
        {
            public string[] locations = { };

            private ICollection<CardLocation> Locations => locations.Select(CardLocationHelpers.FromString).ToArray();

            protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
                => Locations.Any(loc => card.Location == loc);
        }

        public class PositionFitsRestriction : CardRestrictionElement
        {
            public SpaceRestriction spaceRestriction;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                spaceRestriction.Initialize(source: restrictionContext.source, controller: restrictionContext.source.Controller,
                    effect: restrictionContext.subeffect.Effect, subeffect: restrictionContext.subeffect);
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
            public INoActivationContextCardIdentity adjacentTo = new Identities.GamestateCardIdentities.ThisCard();

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                adjacentTo.Initialize(restrictionContext);
            }

            protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
                => adjacentTo.Card.IsAdjacentTo(card);
        }

        public class SubtypesInclude : CardRestrictionElement
        {
            public string[] subtypes;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                if (subtypes == null) throw new System.NotImplementedException($"You forgot to provide a subtypes array!");
            }

            protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
                => subtypes.All(subtype => card.SubtypeText.Contains(subtype));
        }
    }
}