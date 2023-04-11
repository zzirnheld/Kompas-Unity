
using KompasCore.Cards;

namespace KompasCore.Effects.Restrictions.elements
{
    public class CardValueFits : CardRestrictionElement
    {
        public CardValue cardValue;
        public NumberRestriction numberRestriction;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            cardValue.Initialize(initializationContext);
            numberRestriction.Initialize(initializationContext);
        }

        protected override bool IsValidLogic(GameCardBase item, IResolutionContext context)
            => numberRestriction.IsValid(cardValue.GetValueOf(item), context);
    }

    public class Hurt : CardRestrictionElement
    {
        protected override bool IsValidLogic(GameCardBase item, IResolutionContext context)
            => item.Hurt;
    }
}