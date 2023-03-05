using KompasCore.Cards;

namespace KompasCore.Effects.Identities.ActivationContextNumberIdentities
{
    public class FromCardValue : ContextualIdentityBase<int>
    {
        public IIdentity<GameCardBase> card;
        public CardValue cardValue;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            card.Initialize(initializationContext);
            cardValue.Initialize(initializationContext);
        }

        protected override int AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
            => cardValue.GetValueOf(card.From(context, secondaryContext));
    }
}