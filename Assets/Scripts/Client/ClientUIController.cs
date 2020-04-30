using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public Button searchTargetButton;
    public Button cancelSearchButton;
    //effects
    public InputField xInput;
    public GameObject setXView;
    public GameObject declineAnotherTargetView;
    //confirm trigger
    public GameObject ConfirmTriggerView;
    public TMPro.TMP_Text TriggerBlurbText;
    //search
    private List<Card> toSearch;
    private int searchIndex = 0;
    private int numToSearch;
    private ListRestriction searchListRestriction;
    private int numSearched;
    private List<Card> searched;
    //choose effect option
    public GameObject ChooseOptionView;
    public TMPro.TMP_Text ChoiceBlurbText;
    public TMPro.TMP_Dropdown EffectOptionDropdown;

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
        clientGame.clientNotifier.RequestResolveEffect(SelectedCard, index);
    }

    public void ToggleHoldingPriority()
    {
        throw new System.NotImplementedException();
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

    public void ShowOptionalTrigger(Trigger t, int? x)
    {
        TriggerBlurbText.text = t.Blurb;
        ConfirmTriggerView.SetActive(true);
    }

    public void RespondToTrigger(bool answer)
    {
        clientGame.clientNotifier.RequestTriggerReponse(answer);
        ConfirmTriggerView.SetActive(false);
    }

    public void ShowOptions(DummyChooseOptionSubeffect subeff)
    {
        ChoiceBlurbText.text = subeff.ChoiceBlurb;
        EffectOptionDropdown.ClearOptions();
        foreach(string blurb in subeff.OptionBlurbs)
        {
            EffectOptionDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData() { text = blurb });
        }
        ChooseOptionView.SetActive(true);
    }

    public void ChooseOption(int optionIndex)
    {
        clientGame.clientNotifier.RequestChooseEffectOption(optionIndex);
        ChooseOptionView.SetActive(false);
    }
    #endregion effects

    #region import deck
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
    #endregion import deck

    #region search
    public void StartSearch(List<Card> list, ListRestriction listRestriction = null, int numToChoose = 1)
    {
        //if already searching, dont start another search?
        if (toSearch != null && toSearch.Count != 0) return;
        //if the list is empty, don't search
        if (list.Count == 0) return;

        Debug.Log($"Searching a list of {list.Count} cards: {string.Join(",", list.Select(c => c.CardName))}");

        toSearch = list;
        numToSearch = numToChoose;
        searchListRestriction = listRestriction;
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

        Card searchSelected = toSearch[searchIndex];

        if (numToSearch == 1)
        {
            clientGame.clientNotifier.RequestTarget(searchSelected);
            ResetSearch();
        }
        else
        {
            if (searched.Contains(searchSelected)) return;
            searched.Add(searchSelected);

            //TODO a better way to evaluate the list restriction. a partial list might not fit the list restriction, but
            //adding something to that list might make it fit.
            //if there is a list restriction, but it doesn't like this list, refuse to add that card
            /*if(searchListRestriction != null && !searchListRestriction.Evaluate(searched))
            {
                searched.Remove(searchSelected);
                return;
            }*/

            //TODO: if we didn't return, mark that card as selected
            numSearched++;

            //if we were given a maximum number to be searched, and hit that number, no reason to keep asking
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
        cancelSearchButton.gameObject.SetActive(false);
    }


    //TODO rework this to take into get deck from server before searching?
    public void StartDeckSearch()
    {
        StartSearch(clientGame.friendlyDeckCtrl.Deck);
    }

    public void StartDiscardSearch()
    {
        StartSearch(clientGame.friendlyDiscardCtrl.Discard);
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
        if (!(SelectedCard is CharacterCard charCard)) return;

        //get current ones, in case the input fields are empty
        int nToUpdate = charCard.N;
        int eToUpdate = charCard.E;
        int sToUpdate = charCard.S;
        int wToUpdate = charCard.W;

        //if any of the input fields have a value, update the values you want to update 
        if (debugNInputField.text != "") nToUpdate = int.Parse(debugNInputField.text);
        if (debugEInputField.text != "") eToUpdate = int.Parse(debugEInputField.text);
        if (debugSInputField.text != "") sToUpdate = int.Parse(debugSInputField.text);
        if (debugWInputField.text != "") wToUpdate = int.Parse(debugWInputField.text);

        ClientGame.mainClientGame.clientNotifier.RequestSetNESW(charCard, nToUpdate, eToUpdate, sToUpdate, wToUpdate);
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
