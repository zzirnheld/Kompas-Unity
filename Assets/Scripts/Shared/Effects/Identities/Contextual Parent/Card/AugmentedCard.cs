using KompasCore.Cards;

namespace KompasCore.Effects.Identities.ActivationContextCardIdentities
{

    public class AugmentedCard : ContextualIdentityBase<GameCardBase>
    {
        public IIdentity<GameCardBase> ofThisCard;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            ofThisCard.Initialize(initializationContext);
        }

        protected override GameCardBase AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
            => ofThisCard.From(context, secondaryContext).AugmentedCard;
    }
}