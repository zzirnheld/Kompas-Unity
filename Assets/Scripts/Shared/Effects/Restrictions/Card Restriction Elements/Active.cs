using KompasCore.Cards;
using Newtonsoft.Json;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
	public class Active : CardRestrictionElement
	{
		[JsonProperty]
		public bool active = true;

		protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
			=> card.Activated == active;
	}
}