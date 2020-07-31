using KompasClient.Effects;
using KompasClient.GameCore;
using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;
using KompasCore.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace KompasClient.UI
{
    public class ClientUIController : UIController
    {
        public const string FriendlyTurn = "Friendly Turn";
        public const string EnemyTurn = "Enemy Turn";

        public struct SearchData
        {
            public readonly List<GameCard> toSearch;
            public readonly int numToSearch;
            public readonly bool targetingSearch;
            public readonly List<GameCard> searched;

            public SearchData(List<GameCard> toSearch, int numToSearch, bool targetingSearch, List<GameCard> searched)
            {
                this.toSearch = toSearch;
                this.numToSearch = numToSearch;
                this.targetingSearch = targetingSearch;
                this.searched = searched;
            }
        }

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
                $"\n{value + 1} Next Turn";
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
        public TMPro.TMP_Text searchTargetButtonText;
        public TMPro.TMP_Text nSearchText;
        public TMPro.TMP_Text eSearchText;
        public TMPro.TMP_Text sSearchText;
        public TMPro.TMP_Text wSearchText;
        public TMPro.TMP_Text cSearchText;
        public TMPro.TMP_Text aSearchText;
        //effects
        public InputField xInput;
        public GameObject setXView;
        public GameObject declineAnotherTargetView;
        //confirm trigger
        public GameObject ConfirmTriggerView;
        public TMPro.TMP_Text TriggerBlurbText;
        //search
        private int searchIndex = 0;
        private SearchData? currSearchData = null;
        private readonly Stack<SearchData> searchStack = new Stack<SearchData>();
        //choose effect option
        public GameObject ChooseOptionView;
        public TMPro.TMP_Text ChoiceBlurbText;
        public TMPro.TMP_Dropdown EffectOptionDropdown;

        //deck select ui
        public DeckSelectUIController DeckSelectCtrl;
        public GameObject DeckSelectUIParent;
        public GameObject ConnectToServerParent;
        public GameObject DeckSelectorParent;
        public GameObject DeckWaitingParent;
        public GameObject DeckAcceptedParent;
        public GameObject ConnectedWaitingParent;

        private bool ShowEffect(Effect eff) => eff.CanBeActivatedBy(clientGame.Players[0]);

        public override void ShowInfoFor(GameCard card, bool refresh = false)
        {
            base.ShowInfoFor(card);

            if (card != null && card.Effects.Where(eff => ShowEffect(eff)).Any())
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
            var allowedChars = "1234567890.";
            ip = new string(ip.Where(c => allowedChars.Contains(c)).ToArray());

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
            DeckWaitingParent.SetActive(false);
            DeckSelectUIParent.SetActive(true);
        }

        public void AwaitDeckConfirm()
        {
            DeckSelectorParent.SetActive(false);
            DeckAcceptedParent.SetActive(false);
            DeckWaitingParent.SetActive(true);
        }

        public void ShowDeckAcceptedUI()
        {
            DeckSelectorParent.SetActive(false);
            DeckWaitingParent.SetActive(false);
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
            clientGame.clientNotifier.RequestResolveEffect(ShownCard, index);
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
            TriggerBlurbText.text = t.blurb;
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
            foreach (string blurb in subeff.OptionBlurbs)
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
        public void StartSearch(List<GameCard> list, int numToChoose = 1, bool targetingSearch = true) =>
            StartSearch(new SearchData(list, list.Count < numToChoose ? list.Count : numToChoose, targetingSearch, new List<GameCard>()));

        public void StartSearch(SearchData data)
        {
            //if the list is empty, don't search
            if (data.toSearch.Count == 0) return;

            //if should search and already searching, remember current search
            if (currSearchData.HasValue) searchStack.Push(currSearchData.Value);

            currSearchData = data;
            Debug.Log($"Searching a list of {data.toSearch.Count} cards: {string.Join(",", data.toSearch.Select(c => c.CardName))}");

            //initiate search process
            searchIndex = 0;
            cardSearchImage.sprite = currSearchData.Value.toSearch[searchIndex].detailedSprite;
            if (currSearchData.Value.targetingSearch) searchTargetButtonText.text = "Choose";
            else searchTargetButtonText.text = "Cancel";
            cardSearchView.SetActive(true);
        }

        public void SearchSelectedCard()
        {
            //if the list to search through is null, we're not searching atm.
            if (currSearchData == null) return;

            if (!currSearchData.Value.targetingSearch)
            {
                ResetSearch();
                return;
            }

            GameCard searchSelected = currSearchData.Value.toSearch[searchIndex];

            if (currSearchData.Value.numToSearch == 1)
            {
                clientGame.clientNotifier.RequestTarget(searchSelected);
                ResetSearch();
            }
            else if (!currSearchData.Value.searched.Contains(searchSelected))
            {
                currSearchData.Value.searched.Add(searchSelected);

                alreadySelectedText.SetActive(true);

                //if we were given a maximum number to be searched, and hit that number, no reason to keep asking
                if (currSearchData.Value.numToSearch > 1 && currSearchData.Value.searched.Count >= currSearchData.Value.numToSearch)
                {
                    //then send the total list
                    clientGame.clientNotifier.RequestListChoices(currSearchData.Value.searched);
                    //and reset searching
                    ResetSearch();
                }
            }
            else
            {
                //deselect
                currSearchData.Value.searched.Remove(searchSelected);
                alreadySelectedText.SetActive(false);
            }
        }

        /// <summary>
        /// If this is the last search, hides everything. If it's not, moves on to the next search
        /// </summary>
        private void ResetSearch()
        {
            //forget what we were searching through. don't just clear the list because that might clear the actual deck or discard
            currSearchData = null; //thank god for garbage collection lol :upside down smiley:

            if (searchStack.Count == 0) cardSearchView.SetActive(false);
            else StartSearch(searchStack.Pop());
        }

        public void StartDiscardSearch()
        {
            StartSearch(clientGame.friendlyDiscardCtrl.Discard);
        }

        public void NextCardSearch()
        {
            searchIndex++;
            searchIndex %= currSearchData.Value.toSearch.Count;
            SearchShowIndex(searchIndex);
        }

        public void PrevCardSearch()
        {
            searchIndex--;
            if (searchIndex < 0) searchIndex += currSearchData.Value.toSearch.Count;
            SearchShowIndex(searchIndex);
        }

        public void SearchShowIndex(int index)
        {
            var toShow = currSearchData.Value.toSearch[searchIndex];
            cardSearchImage.sprite = toShow.detailedSprite;
            alreadySelectedText.SetActive(currSearchData.Value.searched.Contains(toShow));

            nSearchText.text = $"N\n{toShow.N}";
            eSearchText.text = $"E\n{toShow.E}";
            sSearchText.text = $"S\n{toShow.S}";
            wSearchText.text = $"W\n{toShow.W}";
            cSearchText.text = $"C\n{toShow.C}";
            aSearchText.text = $"A\n{toShow.A}";

            nSearchText.gameObject.SetActive(toShow.CardType == 'C');
            eSearchText.gameObject.SetActive(toShow.CardType == 'C');
            sSearchText.gameObject.SetActive(toShow.CardType == 'C');
            wSearchText.gameObject.SetActive(toShow.CardType == 'C');
            cSearchText.gameObject.SetActive(toShow.CardType == 'S');
            aSearchText.gameObject.SetActive(toShow.CardType == 'A');
        }

        public void SelectShownSearchCard() => HoverOver(currSearchData.Value.toSearch[searchIndex]);
        #endregion

        #region flow control
        public void PassTurn()
        {
            if (clientGame.TurnPlayerIndex == 0)
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
}