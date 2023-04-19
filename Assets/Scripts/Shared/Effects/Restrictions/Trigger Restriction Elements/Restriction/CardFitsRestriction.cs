using System.Collections.Generic;
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
            public IIdentity<IReadOnlyCollection<GameCardBase>> anyOf;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                card?.Initialize(initializationContext);
                anyOf?.Initialize(initializationContext);
                cardRestriction.Initialize(initializationContext);

                if (AllNull(card, anyOf)) throw new System.ArgumentException($"No card to check against restriction in {initializationContext.effect}");
            }

            protected override bool IsValidLogic(TriggeringEventContext context, IResolutionContext secondaryContext)
            {
                var card = this.card.From(context, secondaryContext);
                return cardRestriction.IsValid(card, ContextToConsider(context, secondaryContext));
            }
        }
    }
}