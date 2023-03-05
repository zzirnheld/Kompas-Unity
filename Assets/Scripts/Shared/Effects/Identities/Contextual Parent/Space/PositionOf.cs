using KompasCore.Cards;

namespace KompasCore.Effects.Identities.ActivationContextSpaceIdentities
{
    public class PositionOf : ContextualParentIdentityBase<Space>
    {
        public IIdentity<GameCardBase> whosePosition;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            whosePosition.Initialize(initializationContext);
            base.Initialize(initializationContext);
        }

        protected override Space AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
            => whosePosition.From(context, secondaryContext).Position;
    }
}