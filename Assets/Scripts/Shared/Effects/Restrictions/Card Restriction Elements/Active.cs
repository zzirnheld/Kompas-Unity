using KompasCore.Cards;

namespace KompasCore.Effects.Restrictions.elements
{
    public class Active : CardRestrictionElement
    {
        public bool active = true;

        protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
            => card.Activated == active;
    }
}