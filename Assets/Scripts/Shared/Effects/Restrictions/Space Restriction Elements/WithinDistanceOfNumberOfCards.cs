using KompasCore.Effects.Identities;
using KompasCore.Effects.Identities.Numbers;
using System.Linq;

namespace KompasCore.Effects.Restrictions.elements
{
    public class WithinDistanceOfNumberOfCards : SpaceRestrictionElement
    {
        public CardRestriction cardRestriction;

        public IIdentity<int> numberOfCards = Constant.One;
        public IIdentity<int> distance = Constant.One;

        public bool excludeSelf = true;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            cardRestriction.Initialize(initializationContext);
            numberOfCards.Initialize(initializationContext);
            distance.Initialize(initializationContext);
        }

        protected override bool IsValidLogic(Space space, IResolutionContext context)
        {
            return InitializationContext.game.Cards
                .Where(c => c.DistanceTo(space) < distance.From(context, default))
                .Where(c => cardRestriction.IsValid(c, context))
                .Count() >= numberOfCards.From(context, default);
        }
    }
}