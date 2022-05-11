using KompasCore.Cards;

namespace KompasCore.Effects.Identities
{
    public abstract class ActivationContextSpaceIdentityBase : ContextInitializeableBase, 
        IActivationContextIdentity<Space>
    {
        protected abstract Space AbstractSpaceFrom(ActivationContext context);

        public Space From(ActivationContext context)
        {
            ComplainIfNotInitialized();
            return AbstractSpaceFrom(context);
        }
    }

    namespace ActivationContextSpaceIdentities
    {
        public class PositionOf : ActivationContextSpaceIdentityBase
        {
            public IActivationContextIdentity<GameCardBase> whosePosition;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                whosePosition.Initialize(initializationContext);
                base.Initialize(initializationContext);
            }

            protected override Space AbstractSpaceFrom(ActivationContext context)
                => whosePosition.From(context).Position;
        }

        public class ContextSpace : ActivationContextSpaceIdentityBase
        {
            protected override Space AbstractSpaceFrom(ActivationContext context)
                => context.space;
        }

        public class TwoSpaceIdentity : ActivationContextSpaceIdentityBase
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

            protected override Space AbstractSpaceFrom(ActivationContext context)
            {
                Space first = firstSpace.From(context);
                Space second = secondSpace.From(context);
                return relationship.SpaceFrom(first, second);
            }
        }
    }
}