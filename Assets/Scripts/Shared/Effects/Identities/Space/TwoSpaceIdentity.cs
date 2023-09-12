using Newtonsoft.Json;

namespace KompasCore.Effects.Identities.Spaces
{

	public class TwoSpaceIdentity : ContextualParentIdentityBase<Space>
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> firstSpace;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> secondSpace;

		[JsonProperty(Required = Required.Always)]
		public ITwoSpaceIdentity relationship;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			firstSpace.Initialize(initializationContext);
			secondSpace.Initialize(initializationContext);
			base.Initialize(initializationContext);
		}

		protected override Space AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			Space first = firstSpace.From(context, secondaryContext);
			Space second = secondSpace.From(context, secondaryContext);
			return relationship.SpaceFrom(first, second);
		}
	}
}