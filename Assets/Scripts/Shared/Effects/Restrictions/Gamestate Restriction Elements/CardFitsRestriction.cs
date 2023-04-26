using System.Collections.Generic;
using KompasCore.Cards;
using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions.GamestateRestrictionElements
{
    public class CardFitsRestriction : GamestateRestrictionBase
    {
        public IRestriction<GameCardBase> cardRestriction;
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

        protected override bool IsValidLogic(IResolutionContext context)
        {
            var card = this.card.From(context);
            return cardRestriction.IsValid(card, context);
        }
    }
}