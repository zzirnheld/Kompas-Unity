using KompasCore.Effects.Identities;
using KompasCore.Effects.Identities.GamestateNumberIdentities;
using System.Linq;

namespace KompasCore.Effects.Restrictions.SpaceRestrictionElements
{
    public class WithinDistanceOfNumberOfCards : SpaceRestrictionElement
    {
        public CardRestriction cardRestriction;

        public INoActivationContextIdentity<int> numberOfCards = Constant.One;
        public INoActivationContextIdentity<int> distance = Constant.One;

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
}