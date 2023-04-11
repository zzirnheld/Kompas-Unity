using System.Collections.Generic;
using System.Linq;
using KompasCore.Cards;
using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions.elements
{
    public class AOEContains : CardRestrictionElement
    {
        //If you want to specify the cards that you want to have at least one in AOE using an identity, you can use this one.
        public IIdentity<IReadOnlyCollection<GameCardBase>> cards = new Identities.ManyCards.Board();
        //If you just wanna restrict which of the cards on board have to fit, you can use this one.
        public IRestriction<GameCardBase> cardRestriction = new CardRestriction();

        public bool all = false; //false = any;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            cards.Initialize(initializationContext);
            cardRestriction.Initialize(initializationContext);
        }

        protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
        {
            var wantedCards = cards.From(context, default)
                .Where(c => cardRestriction.IsValid(c, context));

            return all
                ? wantedCards.All(card.CardInAOE)
                : wantedCards.Any(card.CardInAOE);
        }
    }
}