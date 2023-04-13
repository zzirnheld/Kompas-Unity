namespace KompasCore.Effects.Identities.Spaces
{
    public class ApplyDisplacement : ContextualParentIdentityBase<Space>
    {
        public IIdentity<Space> from;
        public IIdentity<Space> displacement;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            from.Initialize(initializationContext);
            displacement.Initialize(initializationContext);
        }

        protected override Space AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
            => from.From(context, secondaryContext) + displacement.From(context, secondaryContext);
    }

    public class Displacement : ContextualParentIdentityBase<Space>
    {
        public IIdentity<Space> from;
        public IIdentity<Space> to;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            from.Initialize(initializationContext);
            to.Initialize(initializationContext);
        }

        protected override Space AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
            => from.From(context, secondaryContext).DisplacementTo(to.From(context, secondaryContext));
    }
}