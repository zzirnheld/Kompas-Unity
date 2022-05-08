using System;

namespace KompasCore.Effects.Identities
{
    public abstract class ActivationContextSpaceIdentity
    {
        private bool initialized;

        protected RestrictionContext RestrictionContext { get; private set; }

        public virtual void Initialize(RestrictionContext restrictionContext)
        {
            RestrictionContext = restrictionContext;

            initialized = true;
        }

        protected abstract Space SpaceFromLogic(ActivationContext context);

        public Space SpaceFrom(ActivationContext context)
            => initialized ? SpaceFromLogic(context)
                : throw new NotImplementedException("You forgot to initialize an ActivationContextSpaceIdentity!");
    }

    public class CardPositionContextSpaceIdentity : ActivationContextSpaceIdentity
    {
        public ActivationContextCardIdentity cardIdentity;

        public override void Initialize(RestrictionContext restrictionContext)
        {
            cardIdentity.Initialize(restrictionContext);
            base.Initialize(restrictionContext);
        }

        protected override Space SpaceFromLogic(ActivationContext context)
            => cardIdentity.CardFrom(context).Position;
    }

    public class ContextSpaceIdentity : ActivationContextSpaceIdentity
    {
        protected override Space SpaceFromLogic(ActivationContext context)
            => context.space;
    }

    public class TwoSpaceIdentityContextSpaceIdentity : ActivationContextSpaceIdentity
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

        protected override Space SpaceFromLogic(ActivationContext context)
        {
            Space first = firstSpaceIdentity.SpaceFrom(context);
            Space second = secondSpaceIdentity.SpaceFrom(context);
            return compositionSpaceIdentity.SpaceFrom(first, second);
        }
    }
}