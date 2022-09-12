using KompasClient.GameCore;
using KompasClient.UI.Search;
using KompasCore.Cards;
using KompasCore.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace KompasClient.UI
{
    public class ClientSidebarCardViewController : SidebarCardViewController
    {
        public ClientGame clientGame;

        [Header("Client-specific UI Controllers")]
        public EffectsParentClientUIController effectsUIController;
        public ClientPipsUIController pipsUIController;

        public SearchUIController searchUICtrl;

        [Tooltip("Whether the card currently being shown is currently selected as a target")]
        public GameObject alreadySelectedMarker;

        //TODO move to own controller
        private readonly List<GameCard> shownUniqueCopies = new List<GameCard>();
        //TODO move to own controller
        private readonly HashSet<GameCard> shownLinkedCards = new HashSet<GameCard>();


        //TODO move these to their own controller
        public GameObject conditionParentObject;
        public GameObject negatedObject;
        public GameObject activatedObject;

        /// <summary>
        /// Whether the current card overrides any attempts to focus on a card, that don't specifically ask to override the focus lock.
        /// Used primarily for searching
        /// </summary>
        private bool focusLocked;

        protected override string EffTextToDisplay
        {
            get
            {
                string effText = base.EffTextToDisplay;
                foreach (string keyword in CardRepository.Keywords)
                {
                    effText = effText.Replace(keyword, $"<link=\"{keyword}\">{keyword}</link>");
                }
                return effText;
            }
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt))
                Show(null);

            //check keywords
            int link = TMP_TextUtilities.FindIntersectingLink(effText, Input.mousePosition, ClientCameraController.Main.mainCamera);
            List<string> reminders = new List<string>();
            if (link != -1)
            {
                var linkInfo = effText.textInfo.linkInfo[link];
                var reminderText = CardRepository.Reminders.KeywordToReminder[linkInfo.GetLinkID()];
                //Debug.Log($"Hovering over {linkInfo.GetLinkID()} with reminder {reminderText}");
                reminders.Add(reminderText);
            }
            reminderTextsUIController.Show(reminders);
            reminderTextsUIController.transform.position = Input.mousePosition;
        }

        public override void Focus(GameCard card) => Focus(card, false);

        /// <summary>
        /// Call with lockFocus = true and card = null to clear out a locked focus and show nothing
        /// </summary>
        /// <param name="card"></param>
        /// <param name="lockFocus"></param>
        public void Focus(GameCard card, bool lockFocus)
        {
            //If we're focus-locked and the most recent card 
            if (focusLocked && !lockFocus)
            {
                Debug.Log($"Client sidebar is currently focus-locked on {card}. Not overriding for {card}");
                return;
            }

            //If the card is null, we're trying to clear 
            focusLocked = lockFocus && card != null;
            base.Focus(card);
        }

        /// <summary>
        /// Stop locking focus on a particular card. We'll still be focused on it, though, until focus shifts to another card
        /// </summary>
        public void ClearFocusLock() => focusLocked = false;

        protected override void DisplayNothing()
        {
            base.DisplayNothing();

            ClearShownUniqueCopies();
            ClearShownCardLinks();

            pipsUIController.HighlightPipsFor(0);
        }

        protected override void Display()
        {
            base.Display();

            //Delegate other responsibilities
            //TODO also on side, not just on right click
            effectsUIController?.ShowEffButtons(ShownCard);
            pipsUIController.HighlightPipsFor(ShownCard);

            ShowUniqueCopies();
            ShowCardLinks();
            conditionParentObject.SetActive(ShownCard.Negated || ShownCard.Activated);
            negatedObject.SetActive(ShownCard.Negated);
            activatedObject.SetActive(ShownCard.Activated);
            alreadySelectedMarker.SetActive(searchUICtrl.CardCurrentlyTargeted(ShownCard));
        }

        private void ClearShownUniqueCopies()
        {
            foreach (var c in shownUniqueCopies) c.CardController.gameCardViewController.ShowUniqueCopy(false);
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
                    copy.CardController.gameCardViewController.ShowUniqueCopy(true);
                    shownUniqueCopies.Add(copy);
                }
            }
        }

        private void ClearShownCardLinks()
        {
            foreach (var c in shownLinkedCards) c.CardController.gameCardViewController.ShowLinkedCard(false);
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
                    card.CardController.gameCardViewController.ShowLinkedCard(true);
                }
            }
        }
    }
}