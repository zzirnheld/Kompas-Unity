
using KompasCore.Effects.Identities;
using KompasCore.Effects.Relationships;

namespace KompasCore.Effects.Restrictions.NumberRestrictionElements
{
	public class Compare : RestrictionBase<int>
	{
		public IIdentity<int> other;
		public INumberRelationship comparison;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			other.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(int item, IResolutionContext context)
			=> comparison.Compare(item, other.From(context, default));
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