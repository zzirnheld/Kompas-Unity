using KompasCore.Cards;

namespace KompasCore.Effects.Identities.ActivationContextPlayerIdentities
{
    public class ControllerOf : ContextualParentIdentityBase<Player>
    {
        public IIdentity<GameCardBase> card;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            card.Initialize(initializationContext);
        }

        protected override Player AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
            => card.From(context, secondaryContext).Controller;
    }
}