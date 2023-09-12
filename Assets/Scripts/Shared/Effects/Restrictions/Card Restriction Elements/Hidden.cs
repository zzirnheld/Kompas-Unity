using KompasCore.Cards;
using Newtonsoft.Json;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
	public class Hidden : CardRestrictionElement
	{
		[JsonProperty]
		public bool hidden = true;

		protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
			=> card.KnownToEnemy == !hidden;
	}
}