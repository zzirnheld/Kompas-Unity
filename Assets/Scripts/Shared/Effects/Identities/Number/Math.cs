using Newtonsoft.Json;

namespace KompasCore.Effects.Identities.Numbers
{
	public class Math : ContextualParentIdentityBase<int>
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<int> number;

		[JsonProperty]
		public int multiplier = 1;
		[JsonProperty]
		public int divisor = 1;
		[JsonProperty]
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