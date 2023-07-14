namespace KompasCore.Effects.Identities.Numbers
{
	public class HandSize : ContextualParentIdentityBase<int>
	{
		public IIdentity<Player> player = new Players.TargetIndex();

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			player.Initialize(initializationContext);
		}

		protected override int AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> player.From(context, secondaryContext).handCtrl.HandSize;
	}
	
	public class HandSizeLimit : ContextualParentIdentityBase<int>
	{
		public IIdentity<Player> player = new Players.TargetIndex();

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			player.Initialize(initializationContext);
		}

		protected override int AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> player.From(context, secondaryContext).HandSizeLimit;
	}
}