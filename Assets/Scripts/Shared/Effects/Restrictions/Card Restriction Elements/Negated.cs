using KompasCore.Cards;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
    public class Negated : CardRestrictionElement
    {
        public bool negated = true;

        protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
            => card.Negated == negated;
    }
}