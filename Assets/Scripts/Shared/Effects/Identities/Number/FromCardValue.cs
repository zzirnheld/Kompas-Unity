using KompasCore.Cards;
using Newtonsoft.Json;

namespace KompasCore.Effects.Identities.Numbers
{
	public class FromCardValue : ContextualParentIdentityBase<int>
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<GameCardBase> card;
		[JsonProperty(Required = Required.Always)]
		public CardValue cardValue;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			card.Initialize(initializationContext);
			cardValue.Initialize(initializationContext);
		}

		protected override int AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> cardValue.GetValueOf(card.From(context, secondaryContext));
	}
}