using KompasCore.Cards;

namespace KompasCore.Effects.Identities.ActivationContextSpaceIdentities
{
    public class PositionOf : ContextualIdentityBase<Space>
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

    public class ContextSpace : ContextualLeafIdentityBase<Space>
    {
        protected override Space AbstractItemFrom(ActivationContext context)
            => context.space;
    }

    public class TargetIndex : ContextlessLeafIdentityBase<Space>
    {
        public int index = -1;

        protected override Space AbstractItem => InitializationContext.effect.GetSpace(index);
    }

    public class TwoSpaceIdentity : ContextualIdentityBase<Space>
    {
        public IIdentity<Space> firstSpace;
        public IIdentity<Space> secondSpace;

        public ITwoSpaceIdentity relationship;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            firstSpace.Initialize(initializationContext);
            secondSpace.Initialize(initializationContext);
            base.Initialize(initializationContext);
        }

        protected override Space AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
        {
            Space first = firstSpace.From(context, secondaryContext);
            Space second = secondSpace.From(context, secondaryContext);
            return relationship.SpaceFrom(first, second);
        }
    }
}