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

        public bool FitsRestriction(GameCardBase card) => initialized ? FitsRestrictionLogic(card)
            : throw new System.NotImplementedException("You failed to initialize a Card Restriction Element");

        protected abstract bool FitsRestrictionLogic(GameCardBase card);
    }

    public class CardExistsRestrictionElement : CardRestrictionElement
    {
        protected override bool FitsRestrictionLogic(GameCardBase card) => card != null;
    }

    public class EnemyCardRestrictionElement : CardRestrictionElement
    {
        protected override bool FitsRestrictionLogic(GameCardBase card)
            => card.Controller != RestrictionContext.source.Controller;
    }

    public class LocationCardRestrictionElement : CardRestrictionElement
    {
        public string[] locations = { };

        private ICollection<CardLocation> Locations => locations.Select(CardLocationHelpers.FromString).ToArray();

        protected override bool FitsRestrictionLogic(GameCardBase card)
            => Locations.Any(loc => card.Location == loc);
    }
}