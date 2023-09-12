
using KompasCore.Cards;
using Newtonsoft.Json;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
	public class CardValueFits : CardRestrictionElement
	{
		[JsonProperty(Required = Required.Always)]
		public CardValue cardValue;
		[JsonProperty(Required = Required.Always)]
		public IRestriction<int> numberRestriction;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			cardValue.Initialize(initializationContext);
			numberRestriction.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(GameCardBase item, IResolutionContext context)
			=> numberRestriction.IsValid(cardValue.GetValueOf(item), context);
	}

	public class Hurt : CardRestrictionElement
	{
		protected override bool IsValidLogic(GameCardBase item, IResolutionContext context)
			=> item.Hurt;
	}
}