﻿using KompasCore.Cards;
using KompasCore.GameCore;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KompasCore.UI
{
    public class UIController : MonoBehaviour
    {
        public const string NoSubtypesUIString = "(No Subtypes)";

        public Toggle debugToggle;
        public GameObject augmentPrefab;
        public GameObject useEffectButtonPrefab;

        //normal UI
        public BoardUIController boardUICtrl;
        //pips
        public TMP_Text friendlyPipsText;
        public TMP_Text enemyPipsText;
        //show selected card data
        public GameObject selectedUIParent;
        public TMP_Text selectedCardNameText;
        public Image selectedCardImage;
        public TMP_Text selectedCardStatsText;
        public TMP_Text selectedCardSubtypesText;
        public TMP_Text selectedCardEffText;
        public GameObject UseEffectParent;
        public GameObject UseEffectGridParent;
        public GameObject AugmentPanelParent;
        public GameObject AugmentGridParent;
        public GameObject negatedParent;
        public GameObject activatedParent;
        //current state text (reminds the player what's happening right now)
        public TMP_Text currentStateText;
        //networking
        public TMP_InputField ipInputField;
        public GameObject networkingParent;

        public WorldCardViewController cardViewController;

        //selection variables
        public GameCard SelectedCard { get; protected set; }

        public GameCard ShownCard { get; protected set; }

        public bool DebugMode { get { return debugToggle.isOn; } }

        //deck search vars
        public List<GameCard> thingToSearch;

        /// <summary>
        /// Updates the info shown in the ui for the given card.
        /// <br></br> If the card passed in is already being shown, but refresh is false, does nothing.
        /// </summary>
        /// <param name="card">The card to show info for.</param>
        /// <param name="refresh">Whether to forcibly refresh all shown info of the card being shown</param>
        /// <returns><see langword="true"/> if the shown info was updated, <see langword="false"/> otherwise.</returns>
        public virtual bool ShowInfoFor(GameCard card, bool refresh = false)
        {
            if (ShownCard == card && !refresh) return false;

            ShownCard = card;
            if (card == null)
            {
                boardUICtrl.ShowNothing();
                return false;
            }
            else
            {
                boardUICtrl.ShowForCard(card, forceRefresh: refresh);
                return true;
            }
        }

        public virtual void Refresh() => ShowInfoFor(ShownCard, refresh: true);

        /// <summary>
        /// updates the ui with the given selection. if the selection is null, hides the ui.
        /// </summary>
        /// <param name="card">make this null to deselect</param>
        /// <param name="fromClick">whether the selecting is from clicking, aka choosing a target</param>
        public virtual void SelectCard(GameCard card, Game.TargetMode targetMode, bool fromClick)
        {
            SelectedCard = card;
            ShowInfoFor(card, refresh: true);
        }

        public void SelectCard(GameCard card, bool fromClick)
        {
            var targetMode = card == null ? Game.TargetMode.Free : card.Game.targetMode;
            SelectCard(card, targetMode, fromClick);
        }

        /// <summary>
        /// Shows information for the <paramref name="card"/>, as you hover over it.
        /// If the <paramref name="card"/> is null, shows information for the currently selected card, if any.
        /// </summary>
        public void HoverOver(GameCard card)
        {
            ShowInfoFor(card ?? SelectedCard);
            cardViewController.Show(card);
        }

        public void RightClick(GameCard card)
        {
            cardViewController.Focus(card);
        }
    }
}