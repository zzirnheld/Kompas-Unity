using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    //debug UI
    public GameObject debugParent;
    public InputField debugNInputField;
    public InputField debugEInputField;
    public InputField debugSInputField;
    public InputField debugWInputField;
    public InputField debugPipsField;
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
    //deck importing
    public InputField deckInputField;
    public Button importDeckButton;
    public Button confirmDeckImportButton;
    //card search
    public GameObject cardSearchView;
    public Image cardSearchImage;
    public Button deckSearchButton;
    public Button discardSearchButton;
    public Button searchDeckToHand;
    public Button searchDiscardToHand;
    public Button cancelDeckSearch;
    public Button cancelDiscardSearch;
    private bool searchingDeck = false;
    private bool searchingDiscard = false;
    private int searchIndex = 0;
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
    private Card selectedCard;
    private CharacterCard selectedChar; //keeps track of last selected character for updating stats in debug, etc.
    private SpellCard selectedSpell;

    private Card hoveredCard;

    public Card SelectedCard { get { return selectedCard; } }
    public CharacterCard SelectedChar { get { return selectedChar; } }
    public SpellCard SelectedSpell { get { return selectedSpell; } }

    public bool DebugMode { get { return debugToggle.isOn; } }

    //deck search vars
    public List<Card> thingToSearch;

    private void Awake()
    {
        deckInputField.lineType = InputField.LineType.MultiLineNewline;
    }

    /// <summary>
    /// updates the ui with the given selection. if the selection is null, hides the ui.
    /// </summary>
    /// <param name="card">make this null to deselect</param>
    public void SelectCard(Card card)
    {
        //if the card is null, deselect everything
        if (card == null)
        {
            selectedUIParent.SetActive(false);
            debugParent.SetActive(false);
            //Debug.Log("Selecting Null");
            selectedCardNameText.text = "No Card Selected";
            selectedCardImage.sprite = Resources.Load<Sprite>("Kompas Circle Background");
            selectedCardStatsText.text = "";
            selectedCardSubtypesText.text = "";
            selectedCardEffText.text = "";
            return;
        }

        selectedUIParent.SetActive(true);
        debugParent.SetActive(true);
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

    public void StopHovering()
    {
        SelectCard(selectedCard);
        hoveredCard = null;
    }

    public void HoverOver(Card card)
    {
        if(card == null)
        {
            StopHovering();
            return;
        }

        if (card == hoveredCard) return;

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

    #region importing/searching deck or discard
    public void ImportDeckPressed()
    {
        deckInputField.gameObject.SetActive(true);
        importDeckButton.gameObject.SetActive(false);
        confirmDeckImportButton.gameObject.SetActive(true);
    }

    public void ConfirmDeckImport()
    {
        string decklist = deckInputField.text;
        deckInputField.gameObject.SetActive(false);
        confirmDeckImportButton.gameObject.SetActive(false);
        importDeckButton.gameObject.SetActive(true);
        //TODO change this to ask the server for import deck
        //if(Game.DEBUG_MODE) ClientGame.mainClientGame.friendlyDeckCtrl.ImportDeck(decklist);
        ClientGame.mainClientGame.clientNetworkCtrl.RequestDecklistImport(decklist);
    }

    //TODO assign all of these methods to buttons
    //TODO rework this to take into get deck from server before searching
    public void StartDeckSearch()
    {
        if (searchingDiscard) return;
        searchingDeck = true;
        cardSearchView.SetActive(true);
        //set buttons to their correct states
        discardSearchButton.gameObject.SetActive(false);
        deckSearchButton.gameObject.SetActive(false);
        searchDeckToHand.gameObject.SetActive(true);
        cancelDeckSearch.gameObject.SetActive(true);
        //initiate search process
        searchIndex = 0;
        if (ClientGame.mainClientGame.friendlyDeckCtrl.DeckSize() > 0)
            cardSearchImage.sprite = ClientGame.mainClientGame.friendlyDeckCtrl.CardAt(0, false, false).DetailedSprite;
        else
            cardSearchImage.sprite = Resources.Load<Sprite>("Card Sprites/Square Kompas Logo");
    }

    public void SearchDeckToHand()
    {
        if (!searchingDeck) return;
        //and request get the card you're currently at for searching
        ClientGame.mainClientGame.clientNetworkCtrl.RequestRehand(ClientGame.mainClientGame.friendlyDeckCtrl.CardAt(searchIndex, false));
        //then be done searching
        EndDeckSearch();
    }

    public void EndDeckSearch()
    {
        cardSearchView.SetActive(false);
        //set buttons to their correct states
        discardSearchButton.gameObject.SetActive(true);
        deckSearchButton.gameObject.SetActive(true);
        searchDeckToHand.gameObject.SetActive(false);
        cancelDeckSearch.gameObject.SetActive(false);
        searchingDeck = false;
    }

    public void StartDiscardSearch()
    {
        if (searchingDeck) return;
        searchingDiscard = true;
        cardSearchView.SetActive(true);
        //set buttons to their correct states
        discardSearchButton.gameObject.SetActive(false);
        deckSearchButton.gameObject.SetActive(false);
        searchDiscardToHand.gameObject.SetActive(true);
        cancelDiscardSearch.gameObject.SetActive(true);
        //intiate process of searching
        searchIndex = 0;
        if (ClientGame.mainClientGame.friendlyDiscardCtrl.DiscardSize() > 0)
            cardSearchImage.sprite = ClientGame.mainClientGame.friendlyDiscardCtrl.CardAt(0, false).DetailedSprite;
        else
            cardSearchImage.sprite = Resources.Load<Sprite>("Card Sprites/Square Kompas Logo");
    }

    public void SearchDiscardToHand()
    {
        if (!searchingDiscard) return;
        //request to add the card that you've searched for
        ClientGame.mainClientGame.clientNetworkCtrl.RequestRehand(ClientGame.mainClientGame.friendlyDiscardCtrl.CardAt(searchIndex, false));
        //and end the search
        EndDiscardSearch();
    }

    public void EndDiscardSearch()
    {
        cardSearchView.SetActive(false);
        //set buttons to their correct states
        discardSearchButton.gameObject.SetActive(true);
        deckSearchButton.gameObject.SetActive(true);
        searchDiscardToHand.gameObject.SetActive(false);
        cancelDiscardSearch.gameObject.SetActive(false);
        //intiate process of searching
        searchingDiscard = false;
    }

    public void NextCardSearch()
    {
        searchIndex++;
        if (searchingDeck)
        {
            if (ClientGame.mainClientGame.friendlyDeckCtrl.DeckSize() > 0)
            {
                searchIndex %= ClientGame.mainClientGame.friendlyDeckCtrl.DeckSize();
                cardSearchImage.sprite = ClientGame.mainClientGame.friendlyDeckCtrl.CardAt(searchIndex, false).DetailedSprite;
            }
        }
        else if (searchingDiscard)
        {
            if (ClientGame.mainClientGame.friendlyDiscardCtrl.DiscardSize() > 0)
            {
                searchIndex %= ClientGame.mainClientGame.friendlyDiscardCtrl.DiscardSize();
                cardSearchImage.sprite = ClientGame.mainClientGame.friendlyDiscardCtrl.CardAt(searchIndex, false).DetailedSprite;
            }
        }
    }

    public void PrevCardSearch()
    {
        searchIndex--;
        if (searchingDeck)
        {
            if (searchIndex < 0) searchIndex += ClientGame.mainClientGame.friendlyDeckCtrl.DeckSize();
            if (ClientGame.mainClientGame.friendlyDeckCtrl.DeckSize() > 0)
                cardSearchImage.sprite = ClientGame.mainClientGame.friendlyDeckCtrl.CardAt(searchIndex, false, false).DetailedSprite;
        }
        else if (searchingDiscard)
        {
            if (searchIndex < 0) searchIndex += ClientGame.mainClientGame.friendlyDiscardCtrl.DiscardSize();
            if (ClientGame.mainClientGame.friendlyDiscardCtrl.DiscardSize() > 0)
                cardSearchImage.sprite = ClientGame.mainClientGame.friendlyDiscardCtrl.CardAt(searchIndex, false).DetailedSprite;
        }
    }
    #endregion

    #region debug
    public void DebugUpdateStats()
    {
        /* old, without checking with server
        if (debugNInputField.text != "")
            SelectedChar.N = Int32.Parse(debugNInputField.text);
        if (debugEInputField.text != "")
            SelectedChar.E = Int32.Parse(debugEInputField.text);
        if (debugSInputField.text != "")
            SelectedChar.S = Int32.Parse(debugSInputField.text);
        if (debugWInputField.text != "")
            SelectedChar.W = Int32.Parse(debugWInputField.text);
            */
        //get current ones, in case the input fields are empty
        int nToUpdate = SelectedChar.N;
        int eToUpdate = SelectedChar.E;
        int sToUpdate = SelectedChar.S;
        int wToUpdate = SelectedChar.W;

        //if any of the input fields have a value, update the values you want to update 
        if (debugNInputField.text != "") nToUpdate = Int32.Parse(debugNInputField.text);
        if (debugEInputField.text != "") eToUpdate = Int32.Parse(debugEInputField.text);
        if (debugSInputField.text != "") sToUpdate = Int32.Parse(debugSInputField.text);
        if (debugWInputField.text != "") wToUpdate = Int32.Parse(debugWInputField.text);

        ClientGame.mainClientGame.clientNetworkCtrl.RequestSetNESW(SelectedChar, nToUpdate, eToUpdate, sToUpdate, wToUpdate);
    }

    public void DebugUpdatePips()
    {
        if (debugPipsField.text != "")
        {
            int toSetPips = Int32.Parse(debugPipsField.text);
            ClientGame.mainClientGame.clientNetworkCtrl.RequestUpdatePips(toSetPips);
        }
    }

    public void DebugUpdateEnemyPips(int num)
    {
        enemyPipsText.text = "Enemy Pips: " + num;
    }
    #endregion debug
    

}
