using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Numbers
{
    public class FromCardValue : ContextualParentIdentityBase<int>
    {
        public IIdentity<GameCardBase> card;
        public CardValue cardValue;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            card.Initialize(initializationContext);
            cardValue.Initialize(initializationContext);
        }

        protected override int AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
            => cardValue.GetValueOf(card.From(context, secondaryContext));
    }
}