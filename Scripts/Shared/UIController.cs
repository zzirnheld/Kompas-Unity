using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    //debug UI


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
            selectedCardSubtypesText.text = selectedChar.Subtypes;
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


}
