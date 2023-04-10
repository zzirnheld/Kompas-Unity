using KompasCore.Cards;
using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions
{

    namespace TriggerRestrictionElements
    {
        public class CardFitsRestriction : TriggerRestrictionElement
        {
            public CardRestriction cardRestriction;
            public IIdentity<GameCardBase> card;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                card.Initialize(initializationContext);
                cardRestriction.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidContext(TriggeringEventContext context, IResolutionContext secondaryContext)
            {
                var card = this.card.From(context, secondaryContext);
                return cardRestriction.IsValid(card, ContextToConsider(context, secondaryContext));
            }
        }
    }
}