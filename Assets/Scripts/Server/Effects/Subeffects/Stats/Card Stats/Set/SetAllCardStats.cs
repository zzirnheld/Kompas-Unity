using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Effects.Identities;
using KompasCore.Effects.Identities.GamestateManyCardsIdentities;
using KompasCore.Effects.Restrictions;
using KompasCore.Effects.Restrictions.CardRestrictionElements;
using System.Collections.Generic;

namespace KompasServer.Effects.Subeffects
{
    public class SetAllCardStats : SetCardStatsOld
    {
        public CardRestriction cardRestriction = new CardRestriction()
        {
            cardRestrictionElements = new CardRestrictionElement[] { new Character() }
        };

        public IIdentity<IReadOnlyCollection<GameCardBase>> cardsSource = new Board();

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            cards ??= new FittingRestriction() {
                cardRestriction = cardRestriction,
                cards = cardsSource
            };
            base.Initialize(eff, subeffIndex);
        }
    }
}