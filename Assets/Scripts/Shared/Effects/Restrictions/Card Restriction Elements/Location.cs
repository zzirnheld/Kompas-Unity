using KompasCore.Cards;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
    public class Location : CardRestrictionElement
    {
        public string[] locations;

        private ICollection<CardLocation> Locations => locations.Select(CardLocationHelpers.FromString).ToArray();

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            if (locations == null) throw new System.ArgumentNullException("locations");
        }

        protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
            => Locations.Any(loc => card.Location == loc);
    }
}