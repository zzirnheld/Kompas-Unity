using KompasCore.Cards;
using Newtonsoft.Json;

namespace KompasCore.Effects.Identities.Cards
{
	public class CardBefore : TriggerContextualCardIdentityBase
	{
		[JsonProperty]
		public bool secondaryCard = false;

		protected override GameCardBase AbstractItemFrom(TriggeringEventContext context)
			=> secondaryCard
				? context.secondaryCardInfoBefore
				: context.mainCardInfoBefore;
	}
}