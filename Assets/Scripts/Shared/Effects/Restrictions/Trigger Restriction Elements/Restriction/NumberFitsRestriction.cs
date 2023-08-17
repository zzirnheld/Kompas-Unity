using KompasCore.Effects.Identities;
using Newtonsoft.Json;

namespace KompasCore.Effects.Restrictions.TriggerRestrictionElements
{
	public class NumberFitsRestriction : TriggerGamestateRestrictionBase
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<int> number;
		[JsonProperty(Required = Required.Always)]
		public IRestriction<int> restriction;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			number.Initialize(initializationContext);
			restriction.Initialize(initializationContext);
		}


		protected override bool IsValidLogic(TriggeringEventContext context, IResolutionContext secondaryContext)
			=> restriction.IsValid(number.From(context, secondaryContext), secondaryContext);
	}
}