using KompasClient.Cards;
using KompasClient.GameCore;
using KompasClient.UI.Search;
using KompasCore.Cards;
using KompasCore.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace KompasClient.UI
{
    public class ClientSidebarCardViewController : SidebarCardViewController
    {
        private const long DoubleClickMillis = 500;

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

        private string oldFileName;

        public VoxelCardUser voxelCardUser;
        public GameObject rawImageShowing;
        public GameObject charOnlyUI;
        public TMP_Text costLabel;

        private GameCard lastFocus;
        private DateTime lastFocusTime;

        protected override bool ShowingInfo { set { base.ShowingInfo = value; rawImageShowing.SetActive(value); } }

        protected override void Update()
        {
            base.Update();
            if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt))
                Show(null);
        }

        public override void Focus(CardBase card) => Focus(card as GameCard, false);

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

            Debug.Log($"Focusing on {card} while lastFocus is {lastFocus} and FocusedCard is {FocusedCard}");
            if (lastFocus != card && FocusedCard == card //If we last focused on a different card, and are now clicking the same card as we did a little moment ago
                && lastFocusTime.AddMilliseconds(DoubleClickMillis) > DateTime.Now)
            {
                //Then lastFocus might be trying to swap with a target card, or attack it.
                boardUIController.Clicked(card.Position, lastFocus);
                Focus(lastFocus);
                return;
            }

            lastFocus = FocusedCard as ClientGameCard;
            lastFocusTime = DateTime.Now;

            //If the card is null, we're trying to clear 
            focusLocked = lockFocus && card != null;
            base.Focus(card);
            (card as ClientGameCard)?.CardController.gameCardViewController.Refresh();
            lastFocus?.CardController.gameCardViewController.Refresh();
            (FocusedCard as ClientGameCard)?.CardController.gameCardViewController.Refresh();
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
            effectsUIController?.ShowEffButtons(ShownGameCard);
            pipsUIController.HighlightPipsFor(ShownGameCard);

            ShowUniqueCopies();
            ShowCardLinks();
            if (null != conditionParentObject)
            {
                conditionParentObject.SetActive(ShownGameCard.Negated || ShownGameCard.Activated);
                negatedObject.SetActive(ShownGameCard.Negated);
                activatedObject.SetActive(ShownGameCard.Activated);
            }
            alreadySelectedMarker.SetActive(searchUICtrl.CardCurrentlyTargeted(ShownGameCard));

            charOnlyUI.SetActive(ShownCard.CardType == 'C');
        }

        private void ClearShownUniqueCopies()
        {
            foreach (var c in shownUniqueCopies) c.CardController.gameCardViewController.ShowUniqueCopy(false);
            shownUniqueCopies.Clear();
        }


        private bool IsFriendlyCopyOf(GameCard a, GameCard b) => a != b && a.Controller == b.Controller && a.CardName == b.CardName;

        private void ShowUniqueCopies()
        {
            ClearShownUniqueCopies();
            if (shownCard.Unique)
            {
                //deal with unique cards
                var copies = clientGame.Cards.Where(c => c.Location == CardLocation.Board && IsFriendlyCopyOf(c, ShownGameCard));
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
            foreach (var link in ShownGameCard.CardLinkHandler.Links)
            {
                foreach (var card in link.CardIDs.Select(clientGame.GetCardWithID))
                {
                    if (card == default) continue;
                    shownLinkedCards.Add(card);
                    card.CardController.gameCardViewController.ShowLinkedCard(true);
                }
            }
        }

        protected override void DisplayCardNumericStats()
        {
            base.DisplayCardNumericStats();
            costLabel.text = ShownCard.CostCardValue.DisplayName;
        }

        protected override string DisplayN(int n) => $"{n}";
        protected override string DisplayE(int e) => $"{e}";
        protected override string DisplayS(int s) => $"{s}";
        protected override string DisplayW(int w) => $"{w}";
        protected override string DisplayC(int c) => $"{c}";
        protected override string DisplayA(int a) => $"{a}";

        protected override void DisplayCardImage(Sprite cardImageSprite)
        {
            if (oldFileName == ShownCard.FileName) return;

            base.DisplayCardImage(cardImageSprite);
            bool isChar = ShownCard.CardType == 'C';
            //TODO split this out if I ever make chars able to become spells or vice versa
            var shownVoxelCardUser = ShownGameCard.CardController.gameCardViewController.voxelCardUser;
            this.voxelCardUser.Set(isChar, true, shownVoxelCardUser.ZoomedTex, shownVoxelCardUser.ZoomedMet);

            oldFileName = ShownCard.FileName;
        }
    }
}