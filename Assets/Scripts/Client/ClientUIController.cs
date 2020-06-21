using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ClientUIController : UIController
{
    public const string FriendlyTurn = "Friendly Turn";
    public const string EnemyTurn = "Enemy Turn";

    public ClientGame clientGame;
    //debug UI 
    /*
    public GameObject debugParent;
    public InputField debugNInputField;
    public InputField debugEInputField;
    public InputField debugSInputField;
    public InputField debugWInputField; */
    public InputField debugPipsField;
    //deck importing
    /*
    public InputField deckInputField;
    public Button importDeckButton;
    public Button confirmDeckImportButton; */

    //gamestate values
    public TMPro.TMP_Text CurrTurnText;
    public GameObject EndTurnButton;

    //current state
    public GameObject CurrStateOverallObj;
    public TMPro.TMP_Text CurrStateText;
    public TMPro.TMP_Text CurrStateBonusText;
    public GameObject CurrStateBonusObj;

    //card search
    public GameObject cardSearchView;
    public Image cardSearchImage;
    /*
    public Button deckSearchButton;
    public Button discardSearchButton;*/
    public Button searchTargetButton;
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
        toSearch = new List<Card>();
    }

    public override void ShowInfoFor(Card card)
    {
        base.ShowInfoFor(card);

        if (card?.Effects != null && card.Effects.Where(eff => eff.Trigger == null).Any())
        {
            var children = new List<GameObject>();
            foreach (Transform child in UseEffectGridParent.transform) children.Add(child.gameObject);
            foreach (var child in children) Destroy(child);

            foreach (var eff in card.Effects)
            {
                if (eff.Trigger != null ||
                    eff.Controller != clientGame.Players[0] || //TODO make this instead be part of activation restriction
                    !eff.ActivationRestriction.Evaluate(clientGame.Players[0])) continue;

                var obj = Instantiate(useEffectButtonPrefab, UseEffectGridParent.transform);
                var btn = obj.GetComponent<ClientUseEffectButtonController>();
                btn.Initialize(eff, this);
            }

            UseEffectParent.SetActive(true);
        }
        else UseEffectParent.SetActive(false);
    }

    public override void SelectCard(Card card, Game.TargetMode targetMode, bool fromClick)
    {
        base.SelectCard(card, targetMode, fromClick);
        if (fromClick && card != null) clientGame.TargetCard(card);
    }

    public void ReselectSelectedCard(bool fromClick)
    {
        SelectCard(SelectedCard, fromClick);
    }

    public void Connect()
    {
        string ip = ipInputField.text;
        if (string.IsNullOrEmpty(ip)) ip = "127.0.0.1";
        try
        {
            clientGame.clientNetworkCtrl.Connect(ip);
            HideNetworkingUI();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to connect, stack trace: {e.StackTrace}");
        }
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

    public void ChangeTurn(int index)
    {
        CurrTurnText.text = index == 0 ? FriendlyTurn : EnemyTurn;
        if (index != 0) EndTurnButton.SetActive(false);
        else EndTurnButton.SetActive(true);
    }

    public void SetCurrState(string primaryState, string secondaryState = "")
    {
        CurrStateOverallObj.SetActive(true);
        CurrStateText.text = primaryState;
        CurrStateBonusText.text = secondaryState;
        if(secondaryState == "") CurrStateBonusObj.SetActive(false);
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
        Debug.Log($"Trying to parse input {xInput.text} for x for effect");
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

    public void ShowEffectOptions(DummyChooseOptionSubeffect subeff)
    {
        ChoiceBlurbText.text = subeff.ChoiceBlurb;
        EffectOptionDropdown.ClearOptions();
        foreach(string blurb in subeff.OptionBlurbs)
        {
            EffectOptionDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData() { text = blurb });
        }
        ChooseOptionView.SetActive(true);
    }

    public void ChooseSelectedEffectOption()
    {
        clientGame.clientNotifier.RequestChooseEffectOption(EffectOptionDropdown.value);
        ChooseOptionView.SetActive(false);
    }
    #endregion effects

    #region search
    public void StartSearch(List<Card> list, ListRestriction listRestriction = null, int numToChoose = 1)
    {
        //if already searching, dont start another search?
        if (toSearch != null && toSearch.Count != 0) return;
        //if the list is empty, don't search
        if (list.Count == 0) return;

        Debug.Log($"Searching a list of {list.Count} cards: {string.Join(",", list.Select(c => c.CardName))}");

        toSearch = list;
        numToSearch = list.Count < numToChoose ? list.Count : numToChoose;
        searchListRestriction = listRestriction;
        numSearched = 0;
        searched = new List<Card>();

        //initiate search process
        searchIndex = 0;
        cardSearchImage.sprite = toSearch[searchIndex].detailedSprite;
        cardSearchView.SetActive(true);
        //set buttons to their correct states
        searchTargetButton.gameObject.SetActive(true);
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

            //TODO: if we didn't return (and so are looking for more cards), mark that card as selected (so the player can tell what they've selected so far)
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
        searchTargetButton.gameObject.SetActive(false);
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

        cardSearchImage.sprite = toSearch[searchIndex].detailedSprite;
    }

    public void PrevCardSearch()
    {
        searchIndex--;
        if (searchIndex < 0) searchIndex += toSearch.Count;

        cardSearchImage.sprite = toSearch[searchIndex].detailedSprite;
    }
    #endregion

    #region flow control
    public void PassTurn()
    {
        if(clientGame.TurnPlayerIndex == 0)
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
