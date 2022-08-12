using KompasCore.Effects;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Cards
{
    public class GameCardCardLinkHandler
    {
        public GameCard Card { get; }

        private readonly List<CardLink> links = new List<CardLink>();
        public IEnumerable<CardLink> Links => links;

        public GameCardCardLinkHandler(GameCard card)
        {
            Card = card;
        }

        private void AddLink(CardLink cardLink)
        {
            links.Add(cardLink);
        }

        public void CreateLink(IEnumerable<GameCard> cards, Effect effect)
        {
            var cardLink = new CardLink(new HashSet<GameCard>(cards), effect);
            foreach(var card in cardLink.Cards)
            {
                card.CardLinkHandler.AddLink(cardLink);
            }
        }

        private void RemoveLink(CardLink cardLink) => links.Remove(cardLink);

        public void RemoveEquivalentLink(IEnumerable<GameCard> cards, Effect effect)
        {
            var equivLink = links.FirstOrDefault(link => link.Matches(cards, effect));
            if (equivLink == default) return;

            foreach(var card in equivLink.Cards)
            {
                card.CardLinkHandler.RemoveLink(equivLink);
            }

            Card.Game.uiCtrl.Refresh();
        }
    }
}