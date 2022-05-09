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
        public class CardPosition : ActivationContextSpaceIdentity
        {
            public ActivationContextCardIdentity cardIdentity;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                cardIdentity.Initialize(restrictionContext);
                base.Initialize(restrictionContext);
            }

            protected override Space AbstractSpaceFrom(ActivationContext context)
                => cardIdentity.CardFrom(context).Position;
        }

        public class ContextSpace : ActivationContextSpaceIdentity
        {
            protected override Space AbstractSpaceFrom(ActivationContext context)
                => context.space;
        }

        public class TwoSpaceIdentity : ActivationContextSpaceIdentity
        {
            public ActivationContextSpaceIdentity firstSpaceIdentity;
            public ActivationContextSpaceIdentity secondSpaceIdentity;

            public ITwoSpaceIdentity compositionSpaceIdentity;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                firstSpaceIdentity.Initialize(restrictionContext);
                secondSpaceIdentity.Initialize(restrictionContext);
                base.Initialize(restrictionContext);
            }

            protected override Space AbstractSpaceFrom(ActivationContext context)
            {
                Space first = firstSpaceIdentity.SpaceFrom(context);
                Space second = secondSpaceIdentity.SpaceFrom(context);
                return compositionSpaceIdentity.SpaceFrom(first, second);
            }
        }
    }
}