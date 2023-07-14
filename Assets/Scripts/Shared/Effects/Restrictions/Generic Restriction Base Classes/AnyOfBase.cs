using System.Linq;

namespace KompasCore.Effects
{
	public abstract class AnyOfBase<RestrictedType> : RestrictionBase<RestrictedType>
	{
		public IRestriction<RestrictedType>[] restrictions;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			foreach (var restriction in restrictions) restriction.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(RestrictedType item, IResolutionContext context)
			=> restrictions.Any(r => r.IsValid(item, context));
	}
}