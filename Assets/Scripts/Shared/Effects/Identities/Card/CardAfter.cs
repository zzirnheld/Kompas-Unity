using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Cards
{
	public class CardAfter : TriggerContextualCardIdentityBase
	{
		public bool secondaryCard;

		protected override GameCardBase AbstractItemFrom(TriggeringEventContext context)
			=> secondaryCard
				? context.SecondaryCardInfoAfter
				: context.MainCardInfoAfter;
	}
}