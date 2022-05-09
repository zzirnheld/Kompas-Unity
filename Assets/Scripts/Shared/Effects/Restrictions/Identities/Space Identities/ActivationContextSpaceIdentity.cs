namespace KompasCore.Effects.Identities
{
    public abstract class ActivationContextSpaceIdentity : ContextInitializeableBase, IContextInitializeable
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
        public class PositionOf : ActivationContextSpaceIdentity
        {
            public ActivationContextCardIdentity whosePosition;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                whosePosition.Initialize(restrictionContext);
                base.Initialize(restrictionContext);
            }

            protected override Space AbstractSpaceFrom(ActivationContext context)
                => whosePosition.CardFrom(context).Position;
        }

        public class ContextSpace : ActivationContextSpaceIdentity
        {
            protected override Space AbstractSpaceFrom(ActivationContext context)
                => context.space;
        }

        public class TwoSpaceIdentity : ActivationContextSpaceIdentity
        {
            public ActivationContextSpaceIdentity firstSpace;
            public ActivationContextSpaceIdentity secondSpace;

            public ITwoSpaceIdentity relationship;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                firstSpace.Initialize(restrictionContext);
                secondSpace.Initialize(restrictionContext);
                base.Initialize(restrictionContext);
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