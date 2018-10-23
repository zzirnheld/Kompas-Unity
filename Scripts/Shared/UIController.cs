using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    //debug UI
    public InputField debugNInputField;
    public InputField debugEInputField;
    public InputField debugSInputField;
    public InputField debugWInputField;
    public InputField debugPipsField;

    //normal UI
    //pips
    public Text friendlyPipsText;
    public Text enemyPipsText;
    //show selected card data
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

    public Card SelectedCard { get { return selectedCard; } }
    public CharacterCard SelectedChar { get { return selectedChar; } }
    public SpellCard SelectedSpell { get { return selectedSpell; } }

    private void Awake()
    {
        deckInputField.lineType = InputField.LineType.MultiLineNewline;
    }

    public void SelectCard(Card card)
    {
        if (card == null) Debug.Log("Selecting Null");
        else Debug.Log("Selecting " + card.CardName);
        selectedCard = card;

        //if the card is null, deselect everything
        if(card == null)
        {
            selectedCardNameText.text = "No Card Selected";
            selectedCardImage.sprite = Resources.Load<Sprite>("Kompas Circle Background");
            selectedCardStatsText.text = "";
            selectedCardSubtypesText.text = "";
            selectedCardEffText.text = "";
            return;
        }

        //do something based on what type the card is
        if(card is CharacterCard)
        {
            selectedChar = card as CharacterCard;
            selectedCardStatsText.text = selectedChar.GetStatsString();
            selectedCardSubtypesText.text = selectedChar.SubtypeText;
        }
        else if(card is SpellCard)
        {
            selectedSpell = selectedCard as SpellCard;
        }

        //set all common values
        selectedCardNameText.text = card.CardName;
        selectedCardImage.sprite = card.DetailedSprite;
        selectedCardEffText.text = card.EffText;

    }

    #region searching deck or discard
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
        Game.mainGame.friendlyDeckCtrl.ImportDeck(decklist);
    }

    //TODO assign all of these methods to buttons
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
        if (Game.mainGame.friendlyDeckCtrl.DeckSize() > 0)
            cardSearchImage.sprite = Game.mainGame.friendlyDeckCtrl.CardAt(0, false, false).DetailedSprite;
        else
            cardSearchImage.sprite = Resources.Load<Sprite>("Card Sprites/Square Kompas Logo");
    }

    public void SearchDeckToHand()
    {
        if (!searchingDeck) return;
        Game.mainGame.friendlyHandCtrl.AddToHand(Game.mainGame.friendlyDeckCtrl.CardAt(searchIndex, true));
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
        if (Game.mainGame.friendlyDiscardCtrl.DiscardSize() > 0)
            cardSearchImage.sprite = Game.mainGame.friendlyDiscardCtrl.CardAt(0, false).DetailedSprite;
        else
            cardSearchImage.sprite = Resources.Load<Sprite>("Card Sprites/Square Kompas Logo");
    }

    public void SearchDiscardToHand()
    {
        if (!searchingDiscard) return;
        Game.mainGame.friendlyHandCtrl.AddToHand(Game.mainGame.friendlyDiscardCtrl.CardAt(searchIndex, true));
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
            if (Game.mainGame.friendlyDeckCtrl.DeckSize() > 0)
            {
                searchIndex %= Game.mainGame.friendlyDeckCtrl.DeckSize();
                cardSearchImage.sprite = Game.mainGame.friendlyDeckCtrl.CardAt(searchIndex, false).DetailedSprite;
            }
        }
        else if (searchingDiscard)
        {
            if (Game.mainGame.friendlyDiscardCtrl.DiscardSize() > 0)
            {
                searchIndex %= Game.mainGame.friendlyDiscardCtrl.DiscardSize();
                cardSearchImage.sprite = Game.mainGame.friendlyDiscardCtrl.CardAt(searchIndex, false).DetailedSprite;
            }
        }
    }

    public void PrevCardSearch()
    {
        searchIndex--;
        if (searchingDeck)
        {
            if (searchIndex < 0) searchIndex += Game.mainGame.friendlyDeckCtrl.DeckSize();
            if (Game.mainGame.friendlyDeckCtrl.DeckSize() > 0)
                cardSearchImage.sprite = Game.mainGame.friendlyDeckCtrl.CardAt(searchIndex, false, false).DetailedSprite;
        }
        else if (searchingDiscard)
        {
            if (searchIndex < 0) searchIndex += Game.mainGame.friendlyDiscardCtrl.DiscardSize();
            if (Game.mainGame.friendlyDiscardCtrl.DiscardSize() > 0)
                cardSearchImage.sprite = Game.mainGame.friendlyDiscardCtrl.CardAt(searchIndex, false).DetailedSprite;
        }
    }
    #endregion

    #region debug
    public void DebugUpdateStats()
    {
        if (debugNInputField.text != "")
            SelectedChar.N = Int32.Parse(debugNInputField.text);
        if (debugEInputField.text != "")
            SelectedChar.E = Int32.Parse(debugEInputField.text);
        if (debugSInputField.text != "")
            SelectedChar.S = Int32.Parse(debugSInputField.text);
        if (debugWInputField.text != "")
            SelectedChar.W = Int32.Parse(debugWInputField.text);
    }

    public void DebugUpdatePips()
    {
        if (debugPipsField.text != "")
        {
            if(Game.mainGame is ClientGame)
                (Game.mainGame as ClientGame).FriendlyPips = Int32.Parse(debugPipsField.text);
            //TODO update if is server
        }
    }

    public void DebugUpdateEnemyPips(int num)
    {
        enemyPipsText.text = "Enemy Pips: " + num;
    }
    #endregion debug

}
