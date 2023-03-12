using KompasCore.Cards;
using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions
{

    namespace TriggerRestrictionElements
    {
        public class CardsMatch : TriggerRestrictionElement
        {
            public IIdentity<GameCardBase> firstCard;
            public IIdentity<GameCardBase> secondCard;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                firstCard.Initialize(initializationContext);
                secondCard.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidContext(ActivationContext context, ActivationContext secondaryContext)
            {
                var first = firstCard.From(context, secondaryContext);
                var second = secondCard.From(context, secondaryContext);
                return first.Card == second.Card;
            }
        }
    }
}