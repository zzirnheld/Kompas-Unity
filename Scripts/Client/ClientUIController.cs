using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientUIController : UIController
{
    public ClientGame clientGame;
    //debug UI
    public GameObject debugParent;
    public InputField debugNInputField;
    public InputField debugEInputField;
    public InputField debugSInputField;
    public InputField debugWInputField;
    public InputField debugPipsField;
    //deck importing
    public InputField deckInputField;
    public Button importDeckButton;
    public Button confirmDeckImportButton;
    //card search
    public GameObject cardSearchView;
    public Image cardSearchImage;
    public Button deckSearchButton;
    public Button discardSearchButton;
    public Button searchToHandButton;
    public Button searchTargetButton;
    public Button cancelSearchButton;

    private List<Card> toSearch;
    private int searchIndex = 0;
    
    private void Awake()
    {
        deckInputField.lineType = InputField.LineType.MultiLineNewline;
        toSearch = new List<Card>();
    }

    public override void SelectCard(Card card, Game.TargetMode targetMode, bool fromClick)
    {
        base.SelectCard(card, targetMode, fromClick);
        if (fromClick && card != null) clientGame.TargetCard(card);
    }

    public void ActivateSelectedCardEff(int index)
    {
        if (selectedCard != null)
            clientGame.clientNetworkCtrl.RequestResolveEffect(selectedCard, index);
    }

    public void ToggleHoldingPriority()
    {
        //TODO
    }

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
        //if(Game.DEBUG_MODE) clientGame.friendlyDeckCtrl.ImportDeck(decklist);
        clientGame.clientNetworkCtrl.RequestDecklistImport(decklist);
    }

    public void StartSearch(List<Card> list, bool targeting)
    {
        Debug.Log("Start search called");
        if (toSearch.Count != 0) return;
        Debug.Log("To search count nonzero");

        toSearch = list;

        //initiate search process
        searchIndex = 0;
        if (toSearch.Count > 0)
        {
            cardSearchImage.sprite = toSearch[searchIndex].DetailedSprite;
            cardSearchView.SetActive(true);
            //set buttons to their correct states
            discardSearchButton.gameObject.SetActive(false);
            deckSearchButton.gameObject.SetActive(false);
            if (targeting)
                searchTargetButton.gameObject.SetActive(true);
            else
                searchToHandButton.gameObject.SetActive(true);
            cancelSearchButton.gameObject.SetActive(true);
        }
        else EndSearch();
    }


    public void ConfirmSearch(bool targeting)
    {
        if (toSearch.Count == 0) return;

        if (targeting)
            clientGame.clientNetworkCtrl.RequestTarget(toSearch[searchIndex]);
        else //TODO remove the option for not a targeting deck search once everything's automated?
            clientGame.clientNetworkCtrl.RequestRehand(toSearch[searchIndex]);

        EndSearch();
    }

    /// <summary>
    /// Hides all buttons relevant to all searches
    /// </summary>
    public void EndSearch()
    {
        //forget what we were searching through. don't just clear the list because that might clear the actual deck or discard
        toSearch = new List<Card>();

        cardSearchView.SetActive(false);
        //set buttons to their correct states
        discardSearchButton.gameObject.SetActive(true);
        deckSearchButton.gameObject.SetActive(true);
        searchTargetButton.gameObject.SetActive(false);
        searchToHandButton.gameObject.SetActive(false);
        cancelSearchButton.gameObject.SetActive(false);
    }


    //TODO rework this to take into get deck from server before searching?
    public void StartDeckSearch()
    {
        StartSearch(clientGame.friendlyDeckCtrl.Deck, false);
    }

    public void StartDiscardSearch()
    {
        StartSearch(clientGame.friendlyDiscardCtrl.Discard, false);
    }

    public void NextCardSearch()
    {
        searchIndex++;
        searchIndex %= toSearch.Count;

        cardSearchImage.sprite = toSearch[searchIndex].DetailedSprite;
    }

    public void PrevCardSearch()
    {
        searchIndex--;
        if (searchIndex < 0) searchIndex += toSearch.Count;

        cardSearchImage.sprite = toSearch[searchIndex].DetailedSprite;
    }
    #endregion

    #region debug
    public void DebugUpdateStats()
    {
        //get current ones, in case the input fields are empty
        int nToUpdate = SelectedChar.N;
        int eToUpdate = SelectedChar.E;
        int sToUpdate = SelectedChar.S;
        int wToUpdate = SelectedChar.W;

        //if any of the input fields have a value, update the values you want to update 
        if (debugNInputField.text != "") nToUpdate = System.Int32.Parse(debugNInputField.text);
        if (debugEInputField.text != "") eToUpdate = System.Int32.Parse(debugEInputField.text);
        if (debugSInputField.text != "") sToUpdate = System.Int32.Parse(debugSInputField.text);
        if (debugWInputField.text != "") wToUpdate = System.Int32.Parse(debugWInputField.text);

        ClientGame.mainClientGame.clientNetworkCtrl.RequestSetNESW(SelectedChar, nToUpdate, eToUpdate, sToUpdate, wToUpdate);
    }

    public void DebugUpdatePips()
    {
        if (debugPipsField.text != "")
        {
            int toSetPips = System.Int32.Parse(debugPipsField.text);
            ClientGame.mainClientGame.clientNetworkCtrl.RequestUpdatePips(toSetPips);
        }
    }

    public void DebugUpdateEnemyPips(int num)
    {
        enemyPipsText.text = "Enemy Pips: " + num;
    }
    #endregion debug
}
