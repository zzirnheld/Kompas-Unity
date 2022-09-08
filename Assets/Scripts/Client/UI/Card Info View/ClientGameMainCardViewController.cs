using KompasClient.GameCore;
using KompasCore.Cards;
using KompasCore.UI;
using System.Collections.Generic;
using UnityEngine;

namespace KompasClient.UI
{
    public class ClientGameMainCardViewController : GameMainCardViewController
    {
        public ClientGame clientGame;

        [Header("Other things to display card-related info")]
        public BoardUIController boardUIController;
        public EffectsParentClientUIController effectsUIController;
        public ReminderTextParentClientUIController reminderTextsUIController;

        public ClientSearchUIController searchUICtrl;

        private readonly List<GameCard> shownUniqueCopies = new List<GameCard>();
        private readonly HashSet<GameCard> shownLinkedCards = new HashSet<GameCard>();

        public ReminderTextsContainer Reminders { get; private set; }

        protected override void DisplayNothing()
        {
            base.DisplayNothing();

            ClearShownUniqueCopies();
            ClearShownCardLinks();

            //Delegate other responsibilities
            boardUIController.ShowNothing();
        }

        protected override void Display()
        {
            base.Display();

            //Delegate other responsibilities
            boardUIController.ShowForCard(ShownCard);
            effectsUIController.ShowEffButtons(ShownCard);
        }

        private void ShowPipsAvailableForCost()
        {
            ClientPlayer clientPlayer = ShownCard.Controller as ClientPlayer;
            int costToHighlight = ShownCard.Location == CardLocation.Hand && ShownCard.Cost <= ShownCard.Controller.Pips
                ? ShownCard.Cost
                : 0;
            clientPlayer.pipsUICtrl.HighlightPipsFor(costToHighlight);
        }

        private void ClearShownUniqueCopies()
        {
            foreach (var c in shownUniqueCopies) c.cardCtrl.ShowUniqueCopy(false);
            shownUniqueCopies.Clear();
        }

        private void ShowUniqueCopies()
        {
            ClearShownUniqueCopies();
            if (CurrShown.Unique)
            {
                //deal with unique cards
                var copies = clientGame.Cards.Where(c => c.Location == CardLocation.Board && c.IsFriendlyCopyOf(CurrShown));
                foreach (var copy in copies)
                {
                    copy.cardCtrl.ShowUniqueCopy(true);
                    shownUniqueCopies.Add(copy);
                }
            }
        }

        private void ClearShownCardLinks()
        {
            foreach (var c in shownLinkedCards) c.cardCtrl.ShowLinkedCard(false);
            shownLinkedCards.Clear();
        }

        private void ShowCardLinks()
        {
            ClearShownCardLinks();
            foreach (var link in CurrShown.CardLinkHandler.Links)
            {
                foreach (var card in link.CardIDs.Select(clientGame.GetCardWithID))
                {
                    if (card == default) continue;
                    shownLinkedCards.Add(card);
                    card.cardCtrl.ShowLinkedCard(true);
                }
            }
        }
    }
}