using KompasClient.GameCore;
using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;
using KompasCore.UI;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KompasClient.UI
{
    public class ClientUIController : UIController
    {
        private const string FriendlyTurn = "Friendly Turn";
        private const string EnemyTurn = "Enemy Turn";

        private const string AwaitingResponseMessage = "Awaiting Response";
        private const string AwaitingEnemyResponse = "Awaiting Enemy Response";

        public ClientGame clientGame;
        //debug UI 
        public InputField debugPipsField;

        //gamestate values
        public TMP_Text CurrTurnText;
        public GameObject EndTurnButton;
        public TMP_Text LeyloadText;

        //current state
        public GameObject CurrStateOverallObj;
        public GameObject CurrStateBonusObj;
        public TMP_Text CurrStateText;
        public TMP_Text CurrStateBonusText;

        //card search
        public GameObject cardSearchView;
        public Image cardSearchImage;
        public GameObject alreadySelectedText;
        public Button searchTargetButton;
        public TMP_Text searchTargetButtonText;
        public TMP_Text nSearchText;
        public TMP_Text eSearchText;
        public TMP_Text sSearchText;
        public TMP_Text wSearchText;
        public TMP_Text cSearchText;
        public TMP_Text aSearchText;
        //effects
        public InputField xInput;
        public GameObject setXView;
        public GameObject declineAnotherTargetView;
        public GameObject declineEffectView;
        public Toggle autodeclineEffects;
        public bool Autodecline => autodeclineEffects.isOn;
        //confirm trigger
        public GameObject ConfirmTriggerView;
        public TMP_Text TriggerBlurbText;
        //search
        private int searchIndex = 0;
        private ClientSearchController.SearchData? CurrSearchData => clientGame.searchCtrl.CurrSearchData;
        //choose effect option
        public ClientChooseOptionUIController chooseOptionUICtrl;

        //stack ui
        public ClientStackPanelController clientStackUICtrl;

        //deck select ui
        public DeckSelectUIController DeckSelectCtrl;
        public GameObject DeckSelectUIParent;
        public GameObject ConnectToServerParent;
        public GameObject DeckSelectorParent;
        public GameObject DeckWaitingParent;
        public GameObject DeckAcceptedParent;
        public GameObject ConnectedWaitingParent;

        //effect option ui
        public TriggerOrderUIController triggerOrderUI;

        //effect activation ui
        public EffectActivatorUIController activatorUICtrl;

        public GameCard CardToActivateEffectsFor
        {
            set => activatorUICtrl.nextShowFor = value;
        }

        public int FriendlyPips
        {
            set => friendlyPipsText.text = $"{value} (+{clientGame.Leyload + (clientGame.FriendlyTurn ? 2 : 1)}) Friendly Pips";
        }

        public int EnemyPips
        {
            set => enemyPipsText.text = $"{value} (+{clientGame.Leyload + (clientGame.FriendlyTurn ? 1 : 2)}) Enemy Pips";
        }
        public int Leyload
        {
            set => LeyloadText.text = $"{value} Pips Leyload";
        }

        public void Update()
        {
            //when the user releaes a right click, show.
            if (Input.GetMouseButtonUp(1)) activatorUICtrl.Show();
        }

        public bool ShowEffect(Effect eff) => eff.CanBeActivatedBy(clientGame.Players[0]);

        public override bool ShowInfoFor(GameCard card, bool refresh = false)
        {
            if (!base.ShowInfoFor(card, refresh)) return false;

            if (card != null && card.Effects.Any(eff => ShowEffect(eff)))
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

            return true;
        }

        public override void SelectCard(GameCard card, Game.TargetMode targetMode, bool fromClick)
        {
            base.SelectCard(card, targetMode, fromClick);
            if (fromClick && card != null) clientGame.searchCtrl.AddTarget(card);
        }

        public void ReselectSelectedCard(bool fromClick) => SelectCard(SelectedCard, fromClick);

        #region connection/game start
        public void Connect(bool acceptEmpty)
        {
            string ip = ipInputField.text;
            if (string.IsNullOrEmpty(ip) && acceptEmpty) ip = "127.0.0.1";
            if (!IPAddress.TryParse(ip, out _)) return;

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

        public void HideConnectUI() => networkingParent.SetActive(false);

        public void ShowConnectedWaitingUI() => ConnectedWaitingParent.SetActive(true);

        public void ShowConnectUI() => networkingParent.SetActive(true);

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

        public void HideGetDecklistUI() => DeckSelectUIParent.SetActive(false);
        #endregion connection/game start

        public void ChangeTurn(int index)
        {
            CurrTurnText.text = index == 0 ? FriendlyTurn : EnemyTurn;
            EndTurnButton.SetActive(index == 0);
        }

        public void SetCurrState(string primaryState, string secondaryState = "")
        {
            CurrStateOverallObj.SetActive(!string.IsNullOrEmpty(primaryState));
            CurrStateText.text = primaryState;
            CurrStateBonusText.text = secondaryState;
            CurrStateBonusObj.SetActive(!string.IsNullOrWhiteSpace(secondaryState));
        }

        #region effects
        public void ActivateSelectedCardEff(int index) => ActivateCardEff(ShownCard, index);

        public void ActivateCardEff(GameCard card, int index)
            => clientGame.clientNotifier.RequestActivateEffect(card, index);

        public void ToggleHoldingPriority()
        {
            throw new System.NotImplementedException();
        }

        public void GetXForEffect() => setXView.SetActive(true);

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

        public void EnableDecliningTarget() => declineAnotherTargetView.SetActive(true);

        public void DisableDecliningTarget() => declineAnotherTargetView.SetActive(false);

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

        public void ShowEffectOptions(string choiceBlurb, string[] optionBlurbs)
            => chooseOptionUICtrl.ShowEffectOptions(choiceBlurb, optionBlurbs);

        public void GetResponse()
        {
            //get response as necessary 
            if (Autodecline)
            {
                DeclineResponse();
                currentStateText.text = AwaitingEnemyResponse;
            }
            else
            {
                declineEffectView.SetActive(true);
                currentStateText.text = AwaitingResponseMessage;
            }

        }

        public void UngetResponse() => declineEffectView.SetActive(false);

        public void DeclineResponse()
        {
            declineEffectView.SetActive(false);
            clientGame.clientNotifier.DeclineResponse();
        }
        #endregion effects

        #region search
        public void StartShowingSearch()
        {
            searchIndex = 0;
            SearchShowIndex(searchIndex);
            if (CurrSearchData.Value.targetingSearch) searchTargetButtonText.text = "Choose";
            else searchTargetButtonText.text = "Cancel";
            cardSearchView.SetActive(true);
        }

        public void SearchSelectedCard()
        {
            //if the list to search through is null, we're not searching atm.
            if (CurrSearchData == null) return;

            if (!CurrSearchData.Value.targetingSearch) clientGame.searchCtrl.ResetSearch();
            else
            {
                GameCard searchSelected = CurrSearchData.Value.toSearch[searchIndex];
                clientGame.searchCtrl.ToggleTarget(searchSelected);
            }
        }

        public void HideSearch() => cardSearchView.SetActive(false);

        public void NextCardSearch()
        {
            searchIndex++;
            searchIndex %= CurrSearchData.Value.toSearch.Length;
            SearchShowIndex(searchIndex);
        }

        public void PrevCardSearch()
        {
            searchIndex--;
            if (searchIndex < 0) searchIndex += CurrSearchData.Value.toSearch.Length;
            SearchShowIndex(searchIndex);
        }

        public void SearchShowIndex(int index)
        {
            var toShow = CurrSearchData.Value.toSearch[index];
            cardSearchImage.sprite = toShow.detailedSprite;
            alreadySelectedText.SetActive(CurrSearchData.Value.searched.Contains(toShow));

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

        public void ReshowSearchShown() => SearchShowIndex(searchIndex);

        public void SelectShownSearchCard() => HoverOver(CurrSearchData.Value.toSearch[searchIndex]);
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
                clientGame.clientNotifier.RequestUpdatePips(toSetPips);
            }
        }

        public void DebugUpdateEnemyPips(int num)
        {
            enemyPipsText.text = "Enemy Pips: " + num;
        }
        #endregion debug
    }
}