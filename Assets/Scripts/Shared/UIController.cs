using KompasCore.Cards;
using KompasCore.GameCore;
using System.Collections.Generic;
using System.Linq;
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
        //pips
        public TMPro.TMP_Text friendlyPipsText;
        public TMPro.TMP_Text enemyPipsText;
        //show selected card data
        public GameObject selectedUIParent;
        public TMPro.TMP_Text selectedCardNameText;
        public Image selectedCardImage;
        public TMPro.TMP_Text selectedCardStatsText;
        public TMPro.TMP_Text selectedCardSubtypesText;
        public TMPro.TMP_Text selectedCardEffText;
        public GameObject UseEffectParent;
        public GameObject UseEffectGridParent;
        public GameObject AugmentPanelParent;
        public GameObject AugmentGridParent;
        public GameObject negatedParent;
        public GameObject activatedParent;
        //current state text (reminds the player what's happening right now)
        public TMPro.TMP_Text currentStateText;
        //networking
        public TMP_InputField ipInputField;
        public GameObject networkingParent;

        //selection variables
        public GameCard SelectedCard { get; protected set; }

        private bool Hovering => hoveredCard != null;
        private GameCard hoveredCard;

        public GameCard ShownCard { get; protected set;}

        public bool DebugMode { get { return debugToggle.isOn; } }

        //deck search vars
        public List<GameCard> thingToSearch;

        public virtual void ShowNothing()
        {
            selectedUIParent.SetActive(false);
            SelectedCard = null;
            hoveredCard = null;
            ShownCard = null;
            //Debug.Log("Selecting Null");
            selectedCardNameText.text = "No Card Selected";
            selectedCardImage.sprite = Resources.Load<Sprite>("Kompas Circle Background");
            selectedCardStatsText.text = "";
            selectedCardSubtypesText.text = "";
            selectedCardEffText.text = "";
        }

        public virtual void ShowInfoFor(GameCard card, bool refresh = false)
        {
            if (ShownCard == card && !refresh) return;

            ShownCard = card;
            if (card == null)
            {
                ShowNothing();
                return;
            }

            //set all common values
            selectedCardStatsText.text = hoveredCard.StatsString;
            selectedCardSubtypesText.text = string.IsNullOrEmpty(card.SubtypeText) ? "(No Subtypes)" : card.SubtypeText;
            selectedCardNameText.text = card.CardName;
            selectedCardImage.sprite = card.detailedSprite;
            selectedCardEffText.text = card.EffText;
            //show if card is negated or activated
            negatedParent.SetActive(card.Negated);
            activatedParent.SetActive(card.Activated);

            if (card?.Augments != null && card.Augments.Any())
            {
                var children = new List<GameObject>();
                foreach (Transform child in AugmentGridParent.transform) children.Add(child.gameObject);
                foreach (var child in children) Destroy(child);

                foreach (var aug in card.Augments)
                {
                    var obj = Instantiate(augmentPrefab, AugmentGridParent.transform);
                    var img = obj.GetComponent<AugmentImageController>();
                    img.Initialize(aug, this);
                }

                AugmentPanelParent.SetActive(true);
            }
            else AugmentPanelParent.SetActive(false);

            selectedUIParent.SetActive(true);
        }

        public void RefreshShownCardInfo() => ShowInfoFor(ShownCard, refresh: true);

        /// <summary>
        /// updates the ui with the given selection. if the selection is null, hides the ui.
        /// </summary>
        /// <param name="card">make this null to deselect</param>
        /// <param name="fromClick">whether the selecting is from clicking, aka choosing a target</param>
        public virtual void SelectCard(GameCard card, Game.TargetMode targetMode, bool fromClick)
        {
            SelectedCard = card;
            ShowInfoFor(card);
        }

        public void SelectCard(GameCard card, bool fromClick)
        {
            if (card == null) SelectCard(null, Game.TargetMode.Free, fromClick);
            else SelectCard(card, card.Game.targetMode, fromClick);
        }

        public void HoverOver(GameCard card)
        {
            hoveredCard = card;
            if (card != null) ShowInfoFor(card);
            else ShowInfoFor(SelectedCard);
        }

        #region updating pips
        public void UpdateFriendlyPips(int num)
        {
            friendlyPipsText.text = $"{num} Friendly Pips";
        }

        public void UpdateEnemyPips(int num)
        {
            enemyPipsText.text = $"{num} Enemy Pips";
        }
        #endregion


    }
}