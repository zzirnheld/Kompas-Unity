using KompasClient.GameCore;
using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;
using KompasCore.UI;
using System;
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

        private const string GameStarting = "Game is Starting";

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
        private string primaryState;
        private string secondaryState;

        //effects
        public InputField xInput;
        public GameObject setXView;
        public GameObject declineAnotherTargetView;
        public GameObject declineEffectView;
        public Toggle autodeclineEffects;
        public bool Autodecline => autodeclineEffects.isOn;
        public TMP_Dropdown autoOptionalEff;
        public const int OptionalEffManual = 0;
        public const int OptionalEffYes = 1;
        public const int OptionalEffNo = 2;
        public int OptionalEffAutoResponse => autoOptionalEff.value;
        //confirm trigger
        public GameObject ConfirmTriggerView;
        public TMP_Text TriggerBlurbText;
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

        //escape menu
        public ClientEscapeMenuUIController escapeMenuUICtrl;

        //card view ui
        public CardInfoViewClientUIController cardInfoViewUICtrl;

        public GameCard CardToActivateEffectsFor
        {
            set => activatorUICtrl.nextShowFor = value;
        }

        public int FriendlyPips
        {
            set => friendlyPipsText.text 
                = $"{value} (+{clientGame.Leyload + (clientGame.FriendlyTurn ? 2 : 1)}) Friendly Pips";
        }

        public int EnemyPips
        {
            set => enemyPipsText.text 
                = $"{value} (+{clientGame.Leyload + (clientGame.FriendlyTurn ? 1 : 2)}) Enemy Pips";
        }
        public int Leyload
        {
            set => LeyloadText.text = $"{value} Pips Leyload";
        }

        private void Update()
        {
            //when the user releaes a right click, show eff activation ui.
            if (Input.GetMouseButtonUp(1)) activatorUICtrl.Show();
            //when the user releases a left click, deselect any applicable effect activation ui
            if (Input.GetMouseButtonUp(0)) activatorUICtrl.CancelIfApplicable();
            //when the user presses escape, show the menu.
            if (Input.GetKeyDown(KeyCode.Escape)) escapeMenuUICtrl.Enable();
        }

        private void Awake()
        {
            SetCurrState(GameStarting);
        }

        public override bool ShowInfoFor(GameCard card, bool refresh = false)
        {
            bool reshow = ShownCard != card || refresh;
            bool success = base.ShowInfoFor(card, refresh);
            if (reshow) cardInfoViewUICtrl.ShowInfoFor(card, refresh);
            return success;
        }

        public bool ShowEffect(Effect eff) => eff.CanBeActivatedBy(clientGame.Players[0]);

        public override void SelectCard(GameCard card, Game.TargetMode targetMode, bool fromClick)
        {
            base.SelectCard(card, targetMode, fromClick);
            if (fromClick && targetMode != Game.TargetMode.Free && card != null) clientGame.searchCtrl.ToggleTarget(card);
        }

        public void ReselectSelectedCard(bool fromClick) => SelectCard(SelectedCard, fromClick);

        #region connection/game start
        public void Connect(bool acceptEmpty)
        {
            string ip = ipInputField.text;
            if (string.IsNullOrEmpty(ip) && acceptEmpty) ip = "127.0.0.1";
            if (!IPAddress.TryParse(ip, out _)) return;

            HideConnectUI();
            clientGame.clientNetworkCtrl.Connect(ip);
        }

        public void HideConnectUI() => networkingParent.SetActive(false);

        public void ShowConnectedWaitingUI() => ConnectedWaitingParent.SetActive(true);

        public void ShowConnectUI() => networkingParent.SetActive(true);

        public void ShowGetDecklistUI()
        {
            ConnectToServerParent.SetActive(false);
            DeckWaitingParent.SetActive(false);
            DeckSelectUIParent.SetActive(true);
            DeckSelectorParent.SetActive(true);
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

        public void SetCurrState(string primaryState, string secondaryState = "", string numTargetsChosen = "")
        {
            this.primaryState = primaryState;
            this.secondaryState = secondaryState;
            CurrStateOverallObj.SetActive(!string.IsNullOrEmpty(primaryState));
            CurrStateText.text = primaryState + numTargetsChosen;
            CurrStateBonusText.text = secondaryState;
            CurrStateBonusObj.SetActive(!string.IsNullOrWhiteSpace(secondaryState));
        }

        public void UpdateCurrState(string primaryState = null, string secondaryState = null, string numTargetsChosen = null)
            => SetCurrState(primaryState ?? this.primaryState, secondaryState ?? this.secondaryState, numTargetsChosen ?? string.Empty);

        #region effects
        public void ActivateSelectedCardEff(int index) => ActivateCardEff(ShownCard, index);

        public void ActivateCardEff(GameCard card, int index)
            => clientGame.clientNotifier.RequestActivateEffect(card, index);

        public void GetXForEffect() => setXView.SetActive(true);

        /// <summary>
        /// Sets the value for X in an effect that uses X
        /// </summary>
        public void SetXForEffect()
        {
            //Debug.Log($"Trying to parse input {xInput.text} for x for effect");
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

        public void ShowOptionalTrigger(Trigger t, bool showX, int x)
        {
            if(t.Effect.Controller.Friendly)

            if (OptionalEffAutoResponse == OptionalEffYes) RespondToTrigger(true);
            else if (OptionalEffAutoResponse == OptionalEffNo) RespondToTrigger(false);
            else
            {
                Debug.Log($"Showing eff response parent");
                TriggerBlurbText.text = showX ? $"{t.Blurb} (X = {x})" : t.Blurb;
                ConfirmTriggerView.SetActive(true);
            }
        }

        public void RespondToTrigger(bool answer)
        {
            clientGame.clientNotifier.RequestTriggerReponse(answer);
            ConfirmTriggerView.SetActive(false);
        }

        public void ShowEffectOptions(string choiceBlurb, string[] optionBlurbs, bool showX, int x)
            => chooseOptionUICtrl.ShowEffectOptions(choiceBlurb, optionBlurbs, showX, x);

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

        //called by the checkbox for auto decline changing state
        public void DeclineResponse(bool b)
        {
            if (b) DeclineResponse();
        }
        #endregion effects


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