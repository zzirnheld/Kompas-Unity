using KompasCore.Cards;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
    public class Hidden : CardRestrictionElement
    {
        public bool hidden = true;

        protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
            => card.KnownToEnemy == !hidden;
    }
}