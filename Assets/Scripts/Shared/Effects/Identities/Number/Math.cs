namespace KompasCore.Effects.Identities.Numbers
{
	public class Math : ContextualParentIdentityBase<int>
	{
		public IIdentity<int> number;

		public int multiplier = 1;
		public int divisor = 1;
		public int modifier = 0;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			number.Initialize(initializationContext);
		}

		protected override int AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> (number.From(context, secondaryContext) * multiplier / divisor) + modifier;
	}
}