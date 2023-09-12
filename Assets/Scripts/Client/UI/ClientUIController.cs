using KompasClient.GameCore;
using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;
using KompasCore.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KompasClient.UI
{
	public class ClientUIController : UIController
	{
		private const string FriendlyTurn = "Friendly";
		private const string EnemyTurn = "Enemy";

		private const string GameStarting = "Game is Starting";

		private const string AwaitingResponseMessage = "Awaiting Response";
		private const string AwaitingEnemyResponse = "Awaiting Enemy Response";

		public const int OptionalEffManual = 0;
		public const int OptionalEffYes = 1;
		public const int OptionalEffNo = 2;

		public ClientGame clientGame;
		public TargetMode TargetMode { get; set; } = TargetMode.Free;

		public override IReminderTextParentController ReminderTextParentUIController => cardInfoViewUIController.ReminderTextsUIController;

		public ConnectionUIController connectionUIController;


		[Header("Card Materials")]
		public Material friendlyCardFrameMaterial;
		public Material enemyCardFrameMaterial;
		[Header("Pips")]
		public TMP_Text friendlyPipsText;
		public TMP_Text enemyPipsText;

		[Header("Turn")]
		public TMP_Text CurrTurnText;
		public Outline currTurnOutline;
		public GameObject EndTurnButton;
		public TMP_Text LeyloadText;

		[Header("Current State")]
		public TMP_Text currentStateText;
		public GameObject CurrStateOverallObj;
		public GameObject CurrStateBonusObj;
		public TMP_Text CurrStateText;
		public TMP_Text CurrStateBonusText;
		private string primaryState;
		private string secondaryState;

		[Header("Effects")]
		public InputField xInput;
		public GameObject setXView;
		public GameObject declineAnotherTargetView;
		public GameObject declineEffectView;
		public Toggle autodeclineEffects;
		public bool Autodecline => autodeclineEffects.isOn;
		public TMP_Dropdown autoOptionalEff;
		public int OptionalEffAutoResponse => autoOptionalEff.value;
		public GameObject detailedEffectsCtrlUIObject;

		[Header("Triggers")]
		private Trigger currOptionalTrigger;
		//confirm trigger
		public GameObject ConfirmTriggerView;
		public TMP_Text TriggerBlurbText;
		//effect ordering
		public TriggerOrderUIController triggerOrderUI;
		//choose effect option
		public ClientChooseOptionUIController chooseOptionUICtrl;

		[Header("Effects Activation")]
		public RightClickCardClientUIController rightClickUIController;

		[Header("Misc")]
		public EscapeMenuUIController escapeMenuUIController;
		public ClientSidebarCardViewController cardInfoViewUIController;
		public override SidebarCardViewController CardViewController => cardInfoViewUIController;
		public ClientSettingsUIController clientUISettingsController;
		public ClientBoardUIController boardUIController;

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
			set => LeyloadText.text = $"{value}";
		}

		private void Update()
		{
			//when the user presses escape, show the menu.
			if (Input.GetKeyDown(KeyCode.Escape)) escapeMenuUIController.Enable();
		}

		private void Awake()
		{
			SetCurrState(GameStarting);
		}

		public void ApplySettings(ClientSettings clientSettings)
		{
			connectionUIController.ApplySettings(clientSettings);
			detailedEffectsCtrlUIObject.SetActive(clientSettings.showAdvancedEffectsSettings);
			Debug.Log($"Friendly {clientSettings.FriendlyColor}");
			friendlyCardFrameMaterial.color = clientSettings.FriendlyColor;
			enemyCardFrameMaterial.color = clientSettings.EnemyColor;
		}
			//TODO if (fromClick && targetMode != Game.TargetMode.Free && card != null) clientGame.searchCtrl.ToggleTarget(card);


		public void ChangeTurn(int index)
		{
			CurrTurnText.text = index == 0 ? FriendlyTurn : EnemyTurn;
			Debug.Log($"{clientGame}, {clientGame?.ClientSettings}, {clientGame?.ClientSettings?.FriendlyColor}");
			currTurnOutline.effectColor = index == 0
				? clientGame.ClientSettings.FriendlyColor
				: clientGame.ClientSettings.EnemyColor;
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
			Debug.Log("Decline another target");
			DisableDecliningTarget();
			clientGame.clientNotifier.DeclineAnotherTarget();
		}

		public void ShowOptionalTrigger(Trigger t, bool showX, int x)
		{
			var blurb = showX ? $"{t.Blurb} (X = {x})" : t.Blurb;

			if (t.Effect.Controller.Friendly)
			{
				if (OptionalEffAutoResponse == OptionalEffYes) RespondToTrigger(true);
				else if (OptionalEffAutoResponse == OptionalEffNo) RespondToTrigger(false);
				else
				{
					//Debug.Log($"Showing eff response parent");
					TriggerBlurbText.text = blurb;
					ConfirmTriggerView.SetActive(true);
					SetCurrState("Choose optional trigger");
					currOptionalTrigger = t;
				}
			}
			else
			{
				SetCurrState("Awaiting other player choice", secondaryState: blurb);
			}
		}

		public void ShowOptionalTriggerSource() => currOptionalTrigger?.Source.CardController.gameCardViewController.ShowPrimaryOfStackable(true);
		public void HideOptionalTriggerSource() => currOptionalTrigger?.Source.CardController.gameCardViewController.ShowPrimaryOfStackable(false);

		public void RespondToTrigger(bool answer)
		{
			HideOptionalTriggerSource();
			clientGame.clientNotifier.RequestTriggerReponse(answer);
			ConfirmTriggerView.SetActive(false);
			currOptionalTrigger = default;
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
	}
}