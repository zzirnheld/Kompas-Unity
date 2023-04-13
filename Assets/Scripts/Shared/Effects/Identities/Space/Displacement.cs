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

        public bool subjective = false;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            from.Initialize(initializationContext);
            to.Initialize(initializationContext);
        }

        protected override Space AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
        {
            var origin = from.From(context, secondaryContext);
            var destination = to.From(context, secondaryContext);

            if (subjective)
            {
                origin = InitializationContext.Controller.SubjectiveCoords(origin);
                destination = InitializationContext.Controller.SubjectiveCoords(destination);
            }

            return origin.DisplacementTo(destination);
        }
    }
}