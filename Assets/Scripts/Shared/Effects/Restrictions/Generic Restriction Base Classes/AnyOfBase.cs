using System.Linq;
using Newtonsoft.Json;

namespace KompasCore.Effects
{
	public abstract class AnyOfBase<RestrictedType> : RestrictionBase<RestrictedType>
	{
		[JsonProperty(Required = Required.Always)]
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