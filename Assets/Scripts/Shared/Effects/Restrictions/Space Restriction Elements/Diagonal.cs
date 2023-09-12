using KompasCore.Effects.Identities;
using Newtonsoft.Json;

namespace KompasCore.Effects.Restrictions.SpaceRestrictionElements
{
	public class SameDiagonal : SpaceRestrictionElement
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> other;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			other.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(Space space, IResolutionContext context)
			=> other.From(context).SameDiagonal(space);
	}
}