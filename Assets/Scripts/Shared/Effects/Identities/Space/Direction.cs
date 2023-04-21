namespace KompasCore.Effects.Identities.Spaces
{
	public class Direction : ContextualParentIdentityBase<Space>
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
			=> from.From(context, secondaryContext).DirectionFromThisTo(to.From(context, secondaryContext));
	}
}