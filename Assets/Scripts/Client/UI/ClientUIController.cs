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

		public const int OptionalEffManual = 0;
		public const int OptionalEffYes = 1;
		public const int OptionalEffNo = 2;

		public ClientGame clientGame;
		public TargetMode TargetMode { get; set; } = TargetMode.Free;

		public override IReminderTextParentController ReminderTextParentUIController => cardInfoViewUIController.ReminderTextsUIController;

		public ConnectionUIController connectionUIController;
		public CurrentStateUIController currentStateUIController;

		[Header("Card Materials")]
		public Material friendlyCardFrameMaterial;
		public Material enemyCardFrameMaterial;

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
		private void Update()
		{
			//when the user presses escape, show the menu.
			if (Input.GetKeyDown(KeyCode.Escape)) escapeMenuUIController.Enable();
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
					currentStateUIController.AwaitingChoice(true, blurb);
					currOptionalTrigger = t;
				}
			}
			else currentStateUIController.AwaitingChoice(false, blurb);
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
			if (Autodecline) DeclineResponse();
			else declineEffectView.SetActive(true);

			currentStateUIController.AwaitingResponse(Autodecline);
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