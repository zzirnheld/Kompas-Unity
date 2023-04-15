using KompasCore.Cards;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
    public class Target : CardRestrictionElement
    {
        protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
            => InitializationContext.effect.CardTargets.Contains(card.Card);
    }
}