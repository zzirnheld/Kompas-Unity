using KompasClient.GameCore;
using KompasCore.Cards;
using KompasCore.UI;
using System.Collections.Generic;
using System.Linq;
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
        public ClientPipsUIController pipsUIController;

        public ClientSearchUIController searchUICtrl;

        private readonly List<GameCard> shownUniqueCopies = new List<GameCard>();
        private readonly HashSet<GameCard> shownLinkedCards = new HashSet<GameCard>();

        public ReminderTextsContainer Reminders { get; private set; }



        public GameObject conditionParentObject;
        public GameObject negatedObject;
        public GameObject activatedObject;

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt))
                Show(null);
        }

        protected override void DisplayNothing()
        {
            base.DisplayNothing();

            ClearShownUniqueCopies();
            ClearShownCardLinks();

            //Delegate other responsibilities
            boardUIController.ShowNothing();
            pipsUIController.HighlightPipsFor(0);
        }

        protected override void Display()
        {
            base.Display();

            //Delegate other responsibilities
            boardUIController.ShowForCard(ShownCard);
            effectsUIController.ShowEffButtons(ShownCard);
            pipsUIController.HighlightPipsFor(ShownCard);


            conditionParentObject.SetActive(ShownCard.Negated || ShownCard.Activated);
            negatedObject.SetActive(ShownCard.Negated);
            activatedObject.SetActive(ShownCard.Activated);
        }

        private void ClearShownUniqueCopies()
        {
            foreach (var c in shownUniqueCopies) c.cardCtrl.ShowUniqueCopy(false);
            shownUniqueCopies.Clear();
        }

        private void ShowUniqueCopies()
        {
            ClearShownUniqueCopies();
            if (ShownCard.Unique)
            {
                //deal with unique cards
                var copies = clientGame.Cards.Where(c => c.Location == CardLocation.Board && c.IsFriendlyCopyOf(ShownCard));
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
            foreach (var link in ShownCard.CardLinkHandler.Links)
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