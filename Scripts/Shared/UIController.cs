using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    //debug UI


    //normal UI
    //pips
    public Text pipsText;
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


    public void SelectCard(Card card)
    {

    }


}
