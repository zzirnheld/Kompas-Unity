namespace KompasCore.Effects.Identities
{
    public interface IActivationContextSpaceIdentity : IContextInitializeable
    {
        public Space SpaceFrom(ActivationContext context);
    }

    public abstract class ActivationContextSpaceIdentityBase : ContextInitializeableBase, IActivationContextSpaceIdentity
    {
        protected abstract Space AbstractSpaceFrom(ActivationContext context);

        public Space SpaceFrom(ActivationContext context)
        {
            ComplainIfNotInitialized();
            return AbstractSpaceFrom(context);
        }
    }

    namespace ActivationContextSpaceIdentities
    {
        public class PositionOf : ActivationContextSpaceIdentityBase
        {
            public IActivationContextCardIdentity whosePosition;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                whosePosition.Initialize(initializationContext);
                base.Initialize(initializationContext);
            }

            protected override Space AbstractSpaceFrom(ActivationContext context)
                => whosePosition.CardFrom(context).Position;
        }

        public class ContextSpace : ActivationContextSpaceIdentityBase
        {
            protected override Space AbstractSpaceFrom(ActivationContext context)
                => context.space;
        }

        public class TwoSpaceIdentity : ActivationContextSpaceIdentityBase
        {
            public IActivationContextSpaceIdentity firstSpace;
            public IActivationContextSpaceIdentity secondSpace;

            public ITwoSpaceIdentity relationship;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                firstSpace.Initialize(initializationContext);
                secondSpace.Initialize(initializationContext);
                base.Initialize(initializationContext);
            }

            protected override Space AbstractSpaceFrom(ActivationContext context)
            {
                Space first = firstSpace.SpaceFrom(context);
                Space second = secondSpace.SpaceFrom(context);
                return relationship.SpaceFrom(first, second);
            }
        }
    }
}