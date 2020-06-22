using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UIController : MonoBehaviour {

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
    //current state text (reminds the player what's happening right now)
    public TMPro.TMP_Text currentStateText;
    //networking
    public TMP_InputField ipInputField;
    public GameObject networkingParent;

    private string currentStateString;
    public string CurrentStateString
    {
        get => currentStateString;
        set
        {
            currentStateString = value;
            currentStateText.text = currentStateString;
        }
    }

    //selection variables
    public Card SelectedCard { get; protected set; }

    private bool hovering = false;
    private Card hoveredCard;

    protected Card shownCard;

    public bool DebugMode { get { return debugToggle.isOn; } }

    //deck search vars
    public List<Card> thingToSearch;

    public virtual void ResetShownInfo() => ShowInfoFor(hovering ? hoveredCard : SelectedCard);

    public virtual void ShowInfoFor(Card card, bool refresh = false)
    {
        if (shownCard == card && !refresh) return;

        shownCard = card;

        hoveredCard = card;
        selectedCardStatsText.text = hoveredCard.StatsString;

        //set all common values
        selectedCardSubtypesText.text = string.IsNullOrEmpty(card.SubtypeText) ? "(No Subtypes)" : card.SubtypeText;
        selectedCardNameText.text = card.CardName;
        selectedCardImage.sprite = card.detailedSprite;
        selectedCardEffText.text = card.EffText;

        if (card?.Augments != null && card.Augments.Any())
        {
            var children = new List<GameObject>();
            foreach (Transform child in AugmentGridParent.transform) children.Add(child.gameObject);
            foreach (var child in children) Destroy(child);

            foreach(var aug in card.Augments)
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

    /// <summary>
    /// updates the ui with the given selection. if the selection is null, hides the ui.
    /// </summary>
    /// <param name="card">make this null to deselect</param>
    /// <param name="fromClick">whether the selecting is from clicking, aka choosing a target</param>
    public virtual void SelectCard(Card card, Game.TargetMode targetMode, bool fromClick)
    {
        //if the card is null, deselect everything
        if (card == null)
        {
            selectedUIParent.SetActive(false);
            //Debug.Log("Selecting Null");
            selectedCardNameText.text = "No Card Selected";
            selectedCardImage.sprite = Resources.Load<Sprite>("Kompas Circle Background");
            selectedCardStatsText.text = "";
            selectedCardSubtypesText.text = "";
            selectedCardEffText.text = "";
            return;
        }

        SelectedCard = card;
        ShowInfoFor(card);
    }

    public void SelectCard(Card card, bool fromClick)
    {
        if (card == null) SelectCard(null, Game.TargetMode.Free, fromClick);
        else SelectCard(card, card.game.targetMode, fromClick);
    }

    public void StopHovering()
    {
        if (!hovering) return;
        SelectCard(SelectedCard, false);
        hoveredCard = null;
        hovering = false;
    }

    public void HoverOver(Card card)
    {
        if(card == null)
        {
            StopHovering();
            return;
        }

        if (card == hoveredCard) return;

        hovering = true;

        ShowInfoFor(card);
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
