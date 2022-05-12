using KompasCore.Cards;

namespace KompasCore.Effects.Identities
{
    namespace ActivationContextSpaceIdentities
    {
        public class PositionOf : ActivationContextIdentityBase<Space>
        {
            public IActivationContextIdentity<GameCardBase> whosePosition;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                whosePosition.Initialize(initializationContext);
                base.Initialize(initializationContext);
            }

            protected override Space AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
                => whosePosition.From(context, secondaryContext).Position;
        }

        public class ContextSpace : ActivationContextIdentityBase<Space>
        {
            protected override Space AbstractItemFrom(ActivationContext context)
                => context.space;
        }

        public class TwoSpaceIdentity : ActivationContextIdentityBase<Space>
        {
            public IActivationContextIdentity<Space> firstSpace;
            public IActivationContextIdentity<Space> secondSpace;

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
}