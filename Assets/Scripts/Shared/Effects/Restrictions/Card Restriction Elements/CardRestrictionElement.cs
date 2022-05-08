using KompasCore.Cards;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Restrictions
{
    public abstract class CardRestrictionElement
    {
        private bool initialized;

        protected RestrictionContext RestrictionContext { get; private set; }

        public virtual void Initialize(RestrictionContext restrictionContext)
        {
            RestrictionContext = restrictionContext;

            initialized = true;
        }

        public bool FitsRestriction(GameCardBase card, ActivationContext context) => initialized ? FitsRestrictionLogic(card, context)
            : throw new System.NotImplementedException("You failed to initialize a Card Restriction Element");

        protected abstract bool FitsRestrictionLogic(GameCardBase card, ActivationContext context);
    }

    public class CardExistsRestrictionElement : CardRestrictionElement
    {
        protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
            => card != null;
    }

    public class EnemyCardRestrictionElement : CardRestrictionElement
    {
        protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
            => card.Controller != RestrictionContext.source.Controller;
    }

    public class LocationCardRestrictionElement : CardRestrictionElement
    {
        public string[] locations = { };

        private ICollection<CardLocation> Locations => locations.Select(CardLocationHelpers.FromString).ToArray();

        protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
            => Locations.Any(loc => card.Location == loc);
    }

    public class PositionCardRestrictionElement : CardRestrictionElement
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
}