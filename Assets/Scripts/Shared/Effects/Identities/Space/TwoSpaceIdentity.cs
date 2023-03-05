namespace KompasCore.Effects.Identities.ActivationContextSpaceIdentities
{

    public class TwoSpaceIdentity : ContextualParentIdentityBase<Space>
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