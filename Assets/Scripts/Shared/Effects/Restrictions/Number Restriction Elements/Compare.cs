
using KompasCore.Effects.Identities;
using KompasCore.Effects.Relationships;
using Newtonsoft.Json;

namespace KompasCore.Effects.Restrictions.NumberRestrictionElements
{
	public class Compare : RestrictionBase<int>
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<int> other;
		[JsonProperty(Required = Required.Always)]
		public INumberRelationship comparison;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			other.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(int item, IResolutionContext context)
			=> comparison.Compare(item, other.From(context));
	}

	public class Positive : Compare
	{
		public override void Initialize(EffectInitializationContext initializationContext)
		{
			other ??= Identities.Numbers.Constant.Zero;
			comparison ??= new Relationships.NumberRelationships.GreaterThan();
			base.Initialize(initializationContext);
		}
	}
}