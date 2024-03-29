using System.Linq;
using KompasServer.Cards;
using Newtonsoft.Json;

namespace KompasCore.Effects.Restrictions.TriggerRestrictionElements
{
	public class TriggerKeyword : TriggerRestrictionBase
	{
		[JsonProperty(Required = Required.Always)]
		public string keyword;

		private IRestriction<TriggeringEventContext> [] elements;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			elements = ServerCardRepository.InstantiateTriggerKeyword(keyword);
		}

		protected override bool IsValidLogic(TriggeringEventContext context, IResolutionContext secondaryContext)
			=> elements.All(tre => tre.IsValid(context, secondaryContext));
	}
}