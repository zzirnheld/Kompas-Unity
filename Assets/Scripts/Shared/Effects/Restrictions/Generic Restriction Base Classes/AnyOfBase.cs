using System.Linq;

namespace KompasCore.Effects
{
	public abstract class AnyOfBase<RestrictedType> : RestrictionBase<RestrictedType>
	{
		public IRestriction<RestrictedType>[] elements;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			foreach (var restriction in elements) restriction.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(RestrictedType item, IResolutionContext context)
			=> elements.Any(r => r.IsValid(item, context));
	}
}