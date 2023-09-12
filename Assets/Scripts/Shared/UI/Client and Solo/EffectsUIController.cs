using KompasClient;
using KompasClient.UI;
using KompasCore.Cards;
using KompasCore.Effects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KompasCore.UI
{
	public class EffectsUIController : MonoBehaviour
	{

		public const int OptionalEffManual = 0;
		public const int OptionalEffYes = 1;
		public const int OptionalEffNo = 2;

		public ClientUIController clientUIController;

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
		//confirm trigger
		public GameObject ConfirmTriggerView;
		public TMP_Text TriggerBlurbText;
		//effect ordering
		public TriggerOrderUIController triggerOrderUI;
		//choose effect option
		public ClientChooseOptionUIController chooseOptionUICtrl;
		private Trigger currOptionalTrigger;

		public void ApplySettings(ClientSettings settings)
		{
			detailedEffectsCtrlUIObject.SetActive(settings.showAdvancedEffectsSettings);
		}

		public void GetXForEffect() => setXView.SetActive(true);

		/// <summary>
		/// Sets the value for X in an effect that uses X
		/// </summary>
		public void SetXForEffect()
		{
			//Debug.Log($"Trying to parse input {xInput.text} for x for effect");
			if (int.TryParse(xInput.text, out int x))
			{
				clientUIController.clientGame.clientNotifier.RequestSetX(x);
				setXView.SetActive(false);
			}
		}

		public void EnableDecliningTarget() => declineAnotherTargetView.SetActive(true);

		public void DisableDecliningTarget() => declineAnotherTargetView.SetActive(false);

		public void DeclineAnotherTarget() //TODO hook up to button
		{
			Debug.Log("Decline another target");
			DisableDecliningTarget();
			clientUIController.clientGame.clientNotifier.DeclineAnotherTarget();
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
					clientUIController.currentStateUIController.AwaitingChoice(true, blurb);
					currOptionalTrigger = t;
				}
			}
			else clientUIController.currentStateUIController.AwaitingChoice(false, blurb);
		}

		public void ShowOptionalTriggerSource() => currOptionalTrigger?.Source.CardController.gameCardViewController.ShowPrimaryOfStackable(true);
		public void HideOptionalTriggerSource() => currOptionalTrigger?.Source.CardController.gameCardViewController.ShowPrimaryOfStackable(false);

		public void RespondToTrigger(bool answer)
		{
			HideOptionalTriggerSource();
			clientUIController.clientGame.clientNotifier.RequestTriggerReponse(answer);
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

			clientUIController.currentStateUIController.AwaitingResponse(Autodecline);
		}

		public void UngetResponse() => declineEffectView.SetActive(false);

		public void DeclineResponse()
		{
			declineEffectView.SetActive(false);
			clientUIController.clientGame.clientNotifier.DeclineResponse();
		}

		//TODO called by the checkbox for auto decline changing state - only matters if add fast effs again
		public void DeclineResponse(bool b)
		{
			if (b) DeclineResponse();
		}

		public void ActivateCardEff(GameCard card, int index)
			=> clientUIController.clientGame.clientNotifier.RequestActivateEffect(card, index);
	}
}