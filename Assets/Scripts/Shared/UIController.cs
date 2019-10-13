using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public Toggle debugToggle;

    //normal UI
    //pips
    public Text friendlyPipsText;
    public Text enemyPipsText;
    //show selected card data
    public GameObject selectedUIParent;
    public Text selectedCardNameText;
    public Image selectedCardImage;
    public Text selectedCardStatsText;
    public Text selectedCardSubtypesText;
    public Text selectedCardEffText;
    //current state text (reminds the player what's happening right now)
    public Text currentStateText;
    private string currentStateString;
    //networking
    public InputField ipInputField;
    public GameObject networkingParent;

    public string CurrentStateString
    {
        get { return currentStateString; }
        set
        {
            currentStateString = value;
            currentStateText.text = currentStateString;
        }
    }

    //selection variables
    protected Card selectedCard;
    private CharacterCard selectedChar; //keeps track of last selected character for updating stats in debug, etc.
    private SpellCard selectedSpell;

    private bool hovering = false;
    private Card hoveredCard;

    public Card SelectedCard { get { return selectedCard; } }
    public CharacterCard SelectedChar { get { return selectedChar; } }
    public SpellCard SelectedSpell { get { return selectedSpell; } }

    public bool DebugMode { get { return debugToggle.isOn; } }

    //deck search vars
    public List<Card> thingToSearch;

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

        selectedUIParent.SetActive(true);
        Debug.Log("Selecting " + card.CardName);
        selectedCard = card;

        //do something based on what type the card is
        if(card is CharacterCard)
        {
            selectedChar = card as CharacterCard;
            selectedCardStatsText.text = selectedChar.GetStatsString();
        }
        else if(card is SpellCard)
        {
            selectedSpell = selectedCard as SpellCard;
            selectedCardStatsText.text = "";
        }

        //set all common values
        selectedCardSubtypesText.text = card.SubtypeText;
        selectedCardNameText.text = card.CardName;
        selectedCardImage.sprite = card.DetailedSprite;
        selectedCardEffText.text = card.EffText;
    }

    public void SelectCard(Card card, bool fromClick)
    {
        if (card == null) SelectCard(card, Game.TargetMode.Free, fromClick);
        else SelectCard(card, card.game.targetMode, fromClick);
    }

    public void StopHovering()
    {
        if (!hovering) return;
        SelectCard(selectedCard, false);
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

        selectedUIParent.SetActive(true);
        hoveredCard = card;

        //do something based on what type the card is
        if (card is CharacterCard hoveredChar)
            selectedCardStatsText.text = hoveredChar.GetStatsString();
        else if (card is SpellCard)
            selectedCardStatsText.text = "";

        //set all common values
        selectedCardSubtypesText.text = card.SubtypeText;
        selectedCardNameText.text = card.CardName;
        selectedCardImage.sprite = card.DetailedSprite;
        selectedCardEffText.text = card.EffText;
    }

    public void HideNetworkingUI()
    {
        networkingParent.SetActive(false);
    }

    #region updating pips
    public void UpdateFriendlyPips(int num)
    {
        friendlyPipsText.text = "Friendly Pips: " + num;
    }

    public void UpdateEnemyPips(int num)
    {
        enemyPipsText.text = "Enemy Pips: " + num;
    }
    #endregion
    

}
