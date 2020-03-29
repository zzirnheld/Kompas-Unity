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
    //effects
    public InputField xInput;
    public GameObject setXView;
    public GameObject declineAnotherTargetView;

    private List<Card> toSearch;
    private int searchIndex = 0;
    private int numToSearch;
    private int numSearched;
    private List<Card> searched;

    //deck select ui
    public DeckSelectUIController DeckSelectCtrl;
    public GameObject DeckSelectUIParent;
    public GameObject ConnectToServerParent;
    
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

    public void Connect()
    {
        string ip = ipInputField.text;
        if (string.IsNullOrEmpty(ip)) ip = "localhost";
        clientGame.clientNetworkCtrl.Connect(ip);
        HideNetworkingUI();
    }

    public void ShowGetDecklistUI()
    {
        ConnectToServerParent.SetActive(false);
        DeckSelectUIParent.SetActive(true);
    }

    //TODO: something for if the decklist is rejected

    public void HideGetDecklistUI()
    {
        DeckSelectUIParent.SetActive(false);
    }

    #region effects
    public void ActivateSelectedCardEff(int index)
    {
        if (selectedCard != null)
            clientGame.clientNotifier.RequestResolveEffect(selectedCard, index);
    }

    public void ToggleHoldingPriority()
    {
        //TODO
    }

    public void GetXForEffect()
    {
        setXView.SetActive(true);
    }

    /// <summary>
    /// Sets the value for X in an effect that uses X
    /// </summary>
    public void SetXForEffect()
    {
        if (int.TryParse(xInput.text, out int x))
        {
            clientGame.clientNotifier.RequestSetX(x);
            setXView.SetActive(false);
        }
    }

    public void EnableDecliningTarget()
    {
        declineAnotherTargetView.SetActive(true);
    }

    public void DisableDecliningTarget()
    {
        declineAnotherTargetView.SetActive(false);
    }

    public void DeclineAnotherTarget()
    {
        DisableDecliningTarget();
        clientGame.clientNotifier.DeclineAnotherTarget();
    }
    #endregion effects

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
        clientGame.clientNotifier.RequestDecklistImport(decklist);
    }

    public void StartSearch(List<Card> list, int numToChoose = 1)
    {
        //if already searching, dont start another search?
        if (toSearch.Count != 0) return;
        //if the list is empty, don't search
        if (list.Count == 0) return;

        Debug.Log($"Searching a list of {list.Count} cards");

        toSearch = list;
        numToSearch = numToChoose;
        numSearched = 0;
        searched = new List<Card>();

        //initiate search process
        searchIndex = 0;
        cardSearchImage.sprite = toSearch[searchIndex].DetailedSprite;
        cardSearchView.SetActive(true);
        //set buttons to their correct states
        discardSearchButton.gameObject.SetActive(false);
        deckSearchButton.gameObject.SetActive(false);
        searchTargetButton.gameObject.SetActive(true);
        cancelSearchButton.gameObject.SetActive(true);
    }


    public void SearchSelectedCard()
    {
        //if the list to search through is null, we're not searching atm.
        if (toSearch == null) return;

        if (numToSearch == 1)
        {
            clientGame.clientNotifier.RequestTarget(toSearch[searchIndex]);
            ResetSearch();
        }
        else
        {
            searched.Add(toSearch[searchIndex]);
            //TODO: mark that card as selected
            numSearched++;
            //if we were given a maximum number to be searched
            if (numToSearch > 1 && numSearched >= numToSearch)
            {
                //then send the total list
                clientGame.clientNotifier.RequestListChoices(searched);
                //and reset searching
                ResetSearch();
            }
        }
    }

    public void EndSearch()
    {
        //if the list to search through is null, we're not searching atm.
        if (toSearch == null) return;

        //if we were told to look for any number of cards, send the final list of cards found
        if(numToSearch == -1) clientGame.clientNotifier.RequestListChoices(searched);
        //otherwise, tell the server that we'd like to cancel searching?
        //if we're required to make a search, the server will insist that yes, actually, i need a search from you
        else clientGame.clientNotifier.RequestCancelSearch();

        ResetSearch();
    }

    /// <summary>
    /// Hides all buttons relevant to all searches
    /// </summary>
    private void ResetSearch()
    {
        //forget what we were searching through. don't just clear the list because that might clear the actual deck or discard
        toSearch = null;

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

    #region flow control
    public void PassTurn()
    {
        if(clientGame.turnPlayer == 0)
        {
            clientGame.clientNotifier.RequestEndTurn();
        }
    }
    #endregion flow control

    #region debug
    public void DebugUpdateStats()
    {
        //get current ones, in case the input fields are empty
        int nToUpdate = SelectedChar.N;
        int eToUpdate = SelectedChar.E;
        int sToUpdate = SelectedChar.S;
        int wToUpdate = SelectedChar.W;

        //if any of the input fields have a value, update the values you want to update 
        if (debugNInputField.text != "") nToUpdate = int.Parse(debugNInputField.text);
        if (debugEInputField.text != "") eToUpdate = int.Parse(debugEInputField.text);
        if (debugSInputField.text != "") sToUpdate = int.Parse(debugSInputField.text);
        if (debugWInputField.text != "") wToUpdate = int.Parse(debugWInputField.text);

        ClientGame.mainClientGame.clientNotifier.RequestSetNESW(SelectedChar, nToUpdate, eToUpdate, sToUpdate, wToUpdate);
    }

    public void DebugUpdatePips()
    {
        if (debugPipsField.text != "")
        {
            int toSetPips = int.Parse(debugPipsField.text);
            ClientGame.mainClientGame.clientNotifier.RequestUpdatePips(toSetPips);
        }
    }

    public void DebugUpdateEnemyPips(int num)
    {
        enemyPipsText.text = "Enemy Pips: " + num;
    }
    #endregion debug
}
