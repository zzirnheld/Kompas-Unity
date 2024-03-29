using Newtonsoft.Json;

namespace KompasCore.Effects.Identities.Spaces
{
	public class Multiply : ContextualParentIdentityBase<Space>
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> toMultiply;

		[JsonProperty]
		public int multiplier = 1;
		[JsonProperty]
		public int xMultiplier = 1;
		[JsonProperty]
		public int yMultiplier = 1;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			toMultiply.Initialize(initializationContext);
		}

		protected override Space AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			var space = toMultiply.From(context, secondaryContext);
			space *= multiplier;
			space.x *= xMultiplier;
			space.y *= yMultiplier;
			return space;
		}
	}
}