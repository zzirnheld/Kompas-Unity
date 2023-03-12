using KompasCore.Cards;
using System.Linq;

namespace KompasCore.Effects.Restrictions
{
    public abstract class CardRestrictionElement : ContextInitializeableBase
    {
        public bool FitsRestriction(GameCardBase card, ActivationContext context)
        {
            ComplainIfNotInitialized();

            return card != null && FitsRestrictionLogic(card, context);
        }

        protected abstract bool FitsRestrictionLogic(GameCardBase card, ActivationContext context);
    }

    namespace CardRestrictionElements
    {

        public class Not : CardRestrictionElement
        {
            public CardRestrictionElement element;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                element.Initialize(initializationContext);
            }

            protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
                => !element.FitsRestriction(card, context);
        }

        public class CardExists : CardRestrictionElement
        {
            protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
                => card != null;
        }

        public class Avatar : CardRestrictionElement
        {
            protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
                => card.IsAvatar;
        }

        public class Summoned : CardRestrictionElement
        {
            protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
                => card.Summoned;
        }
    }
}