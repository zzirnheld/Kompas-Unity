using KompasCore.Cards;
using Newtonsoft.Json;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
	public class Negated : CardRestrictionElement
	{
		[JsonProperty]
		public bool negated = true;

		protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
			=> card.Negated == negated;
	}
}