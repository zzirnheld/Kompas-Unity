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

        public void AddLink(CardLink cardLink)
        {
            links.Add(cardLink);
        }

        public void CreateLink(IEnumerable<int> cardIDs, Effect effect)
        {
            var cardLink = new CardLink(new HashSet<int>(cardIDs), effect);
            foreach(var card in cardIDs.Select(Card.Game.GetCardWithID))
            {
                card?.CardLinkHandler.AddLink(cardLink);
            }
        }

        private void RemoveLink(CardLink cardLink) => links.Remove(cardLink);

        public void RemoveEquivalentLink(IEnumerable<int> cardIDs, Effect effect)
        {
            var equivLink = links.FirstOrDefault(link => link.Matches(cardIDs, effect));
            if (equivLink == default) return;

            foreach(var card in equivLink.CardIDs.Select(Card.Game.GetCardWithID))
            {
                card?.CardLinkHandler.RemoveLink(equivLink);
            }

            Card.Game.UIController.cardViewController.Refresh();
        }
    }
}