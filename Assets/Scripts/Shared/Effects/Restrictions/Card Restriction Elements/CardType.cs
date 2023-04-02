using KompasCore.Cards;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
    public class CardType : CardRestrictionElement
    {
        public char cardType;

        protected override bool FitsRestrictionLogic(GameCardBase card, IResolutionContext context)
            => card.CardType == cardType;
    }

    public class Character : CardType
    {
        public override void Initialize(EffectInitializationContext initializationContext)
        {
            cardType = 'C';
            base.Initialize(initializationContext);
        }
    }

    public class Spell : CardType
    {
        public override void Initialize(EffectInitializationContext initializationContext)
        {
            cardType = 'S';
            base.Initialize(initializationContext);
        }
    }

    public class Augment : CardType
    {
        public override void Initialize(EffectInitializationContext initializationContext)
        {
            cardType = 'A';
            base.Initialize(initializationContext);
        }
    }
}