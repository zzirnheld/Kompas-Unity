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
    public InputField debugPipsField;

    //gamestate values
    public TMPro.TMP_Text CurrTurnText;
    public GameObject EndTurnButton;
    public TMPro.TMP_Text LeyloadText;
    public int Leyload
    {
        set => LeyloadText.text = $"{value} Pips Leyload," +
            $"\n{value + (clientGame.TurnPlayerIndex == clientGame.FirstTurnPlayer ? 0 : 1)} Next Turn";
    }

    //current state
    public GameObject CurrStateOverallObj;
    public TMPro.TMP_Text CurrStateText;
    public TMPro.TMP_Text CurrStateBonusText;
    public GameObject CurrStateBonusObj;

    //card search
    public GameObject cardSearchView;
    public Image cardSearchImage;
    public GameObject alreadySelectedText;
    public Button searchTargetButton;
    //effects
    public InputField xInput;
    public GameObject setXView;
    public GameObject declineAnotherTargetView;
    //confirm trigger
    public GameObject ConfirmTriggerView;
    public TMPro.TMP_Text TriggerBlurbText;
    //search
    private List<GameCard> toSearch;
    private int searchIndex = 0;
    private int numToSearch;
    private ListRestriction searchListRestriction;
    private int numSearched;
    private List<GameCard> searched;
    //choose effect option
    public GameObject ChooseOptionView;
    public TMPro.TMP_Text ChoiceBlurbText;
    public TMPro.TMP_Dropdown EffectOptionDropdown;

    //deck select ui
    public DeckSelectUIController DeckSelectCtrl;
    public GameObject DeckSelectUIParent;
    public GameObject ConnectToServerParent;
    public GameObject DeckSelectorParent;
    public GameObject DeckAcceptedParent;
    public GameObject ConnectedWaitingParent;
    
    private void Awake()
    {
        toSearch = new List<GameCard>();
    }

    private bool ShowEffect(Effect eff)
    {
        return eff.Trigger == null &&
                    eff.Source.Controller == clientGame.Players[0] && //TODO make this instead be part of activation restriction
                    eff.ActivationRestriction.Evaluate(clientGame.Players[0]);
    }

    public override void ShowInfoFor(GameCard card, bool refresh = false)
    {
        base.ShowInfoFor(card);

        if (card?.Effects != null && card.Effects.Where(eff => ShowEffect(eff)).Any())
        {
            var children = new List<GameObject>();
            foreach (Transform child in UseEffectGridParent.transform) children.Add(child.gameObject);
            foreach (var child in children) Destroy(child);

            foreach (var eff in card.Effects)
            {
                if (!ShowEffect(eff)) continue;

                var obj = Instantiate(useEffectButtonPrefab, UseEffectGridParent.transform);
                var btn = obj.GetComponent<ClientUseEffectButtonController>();
                btn.Initialize(eff, this);
            }

            UseEffectParent.SetActive(true);
            selectedUIParent.SetActive(false);
            selectedUIParent.SetActive(true);
        }
        else UseEffectParent.SetActive(false);
    }

    public override void SelectCard(GameCard card, Game.TargetMode targetMode, bool fromClick)
    {
        base.SelectCard(card, targetMode, fromClick);
        if (fromClick && card != null) clientGame.TargetCard(card);
    }

    public void ReselectSelectedCard(bool fromClick)
    {
        SelectCard(SelectedCard, fromClick);
    }

    #region connection/game start
    public void Connect()
    {
        string ip = ipInputField.text;
        if (string.IsNullOrEmpty(ip)) ip = "127.0.0.1";
        try
        {
            HideConnectUI();
            clientGame.clientNetworkCtrl.Connect(ip);
            ShowConnectedWaitingUI();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to connect, stack trace: {e.StackTrace}");
            ShowConnectUI();
        }
    }

    public void HideConnectUI()
    {
        networkingParent.SetActive(false);
    }

    public void ShowConnectedWaitingUI()
    {
        ConnectedWaitingParent.SetActive(true);
    }

    public void ShowConnectUI()
    {
        networkingParent.SetActive(true);
    }

    public void ShowGetDecklistUI()
    {
        ConnectToServerParent.SetActive(false);
        DeckSelectUIParent.SetActive(true);
    }

    public void ShowDeckAcceptedUI()
    {
        DeckSelectorParent.SetActive(false);
        DeckAcceptedParent.SetActive(true);
    }

    public void HideGetDecklistUI()
    {
        DeckSelectUIParent.SetActive(false);
    }
    #endregion connection/game start

    public void ChangeTurn(int index)
    {
        CurrTurnText.text = index == 0 ? FriendlyTurn : EnemyTurn;
        EndTurnButton.SetActive(index == 0);
    }

    public void SetCurrState(string primaryState, string secondaryState = "")
    {
        CurrStateOverallObj.SetActive(true);
        CurrStateText.text = primaryState;
        CurrStateBonusText.text = secondaryState;
        CurrStateBonusObj.SetActive(!string.IsNullOrWhiteSpace(secondaryState));
    }

    #region effects
    public void ActivateSelectedCardEff(int index)
    {
        clientGame.clientNotifier.RequestResolveEffect(shownCard, index);
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
    public void StartSearch(List<GameCard> list, ListRestriction listRestriction = null, int numToChoose = 1)
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
        searched = new List<GameCard>();

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

        GameCard searchSelected = toSearch[searchIndex];

        if (numToSearch == 1)
        {
            clientGame.clientNotifier.RequestTarget(searchSelected);
            ResetSearch();
        }
        else if(!searched.Contains(searchSelected))
        {
            searched.Add(searchSelected);

            alreadySelectedText.SetActive(true);

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
        else
        {
            //deselect
            searched.Remove(searchSelected);
            numSearched--;
            alreadySelectedText.SetActive(false);
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
        alreadySelectedText.SetActive(searched.Contains(toSearch[searchIndex]));
    }

    public void PrevCardSearch()
    {
        searchIndex--;
        if (searchIndex < 0) searchIndex += toSearch.Count;

        cardSearchImage.sprite = toSearch[searchIndex].detailedSprite;
        alreadySelectedText.SetActive(searched.Contains(toSearch[searchIndex]));
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
