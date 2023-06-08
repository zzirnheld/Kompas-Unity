using KompasCore.Cards;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
	public class Avatar : CardRestrictionElement
	{
		protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
			=> card.IsAvatar;
	}

	public class Summoned : CardRestrictionElement
	{
		protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
			=> card.Summoned;
	}
}