using KompasCore.Cards;
using System.Collections.Generic;

namespace KompasCore.Effects.Identities.ManyCards
{
    public class Deck : ContextlessLeafIdentityBase<IReadOnlyCollection<GameCardBase>>
    {
        public bool friendly = true;
        public bool enemy = true;

        protected override IReadOnlyCollection<GameCardBase> AbstractItem
        {
            get
            {
                var cards = new List<GameCardBase>();
                if (friendly) cards.AddRange(InitializationContext.Controller.deckCtrl.Deck);
                if (enemy) cards.AddRange(InitializationContext.Controller.Enemy.deckCtrl.Deck);
                return cards;
            }
        }
    }
}