using KompasCore.Cards;
using KompasServer.Effects.Subeffects;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
    public class SpaceRestrictionWillBeValid : CardRestrictionElement
    {
        public int subeffectIndex;

        protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
            => InitializationContext.effect.Subeffects[subeffectIndex] is SpaceTarget spaceTgtSubeff
                    && spaceTgtSubeff.WillBePossibleIfCardTargeted(theoreticalTarget: card?.Card);
    }
}