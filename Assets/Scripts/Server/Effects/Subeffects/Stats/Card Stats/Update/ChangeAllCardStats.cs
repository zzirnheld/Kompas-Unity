using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Effects.Identities;
using KompasCore.Effects.Identities.ManyCards;
using KompasCore.Effects.Restrictions.CardRestrictionElements;
using System.Collections.Generic;

namespace KompasServer.Effects.Subeffects
{
    public class ChangeAllCardStats : ChangeCardStats
    {
        //default to making sure things are characters before changing their stats
        public IRestriction<GameCardBase> cardRestriction = new CardRestriction()
        {
            elements = new IRestriction<GameCardBase>[] { new Character() }
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