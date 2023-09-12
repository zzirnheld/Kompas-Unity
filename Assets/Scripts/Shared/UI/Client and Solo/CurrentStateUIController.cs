using KompasClient.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KompasCore.UI
{
	public class CurrentStateUIController : MonoBehaviour
	{
		private const string FriendlyTurn = "Friendly";
		private const string EnemyTurn = "Enemy";
		private const string GameStarting = "Game is Starting";

		private const string AwaitingResponseMessage = "Awaiting Response";
		private const string AwaitingEnemyResponse = "Awaiting Enemy Response";

		private const string AwaitingYourChoice = "Choose an effect option";
		private const string AwaitingEnemyChoice = "Awaiting Enemy Choice";

		[Header("Bookkeeping")]
		public ClientUIController clientUIController;

		[Header("Turn")]
		public TMP_Text currTurnText;
		public Outline currTurnOutline;
		public GameObject endTurnButton;
		public TMP_Text leyloadText;

		[Header("Current State")]
		public GameObject currStateOverallObj;
		public GameObject currStateBonusObj;
		public TMP_Text currStateText;
		public TMP_Text currStateBonusText;
		
		private string primaryState;
		private string secondaryState;


		public int Leyload
		{
			set => leyloadText.text = $"{value}";
		}

		private void Awake()
		{
			SetCurrState(GameStarting);
		}

		public void ChangeTurn(int index)
		{
			currTurnText.text = index == 0 ? FriendlyTurn : EnemyTurn;
			Debug.Log($"{clientUIController.clientGame}, {clientUIController.clientGame?.ClientSettings}, {clientUIController.clientGame?.ClientSettings?.FriendlyColor}");
			currTurnOutline.effectColor = index == 0
				? clientUIController.clientGame.ClientSettings.FriendlyColor
				: clientUIController.clientGame.ClientSettings.EnemyColor;
			endTurnButton.SetActive(index == 0);
		}

		public void SetCurrState(string primaryState, string secondaryState = "", string numTargetsChosen = "")
		{
			this.primaryState = primaryState;
			this.secondaryState = secondaryState;
			currStateOverallObj.SetActive(!string.IsNullOrEmpty(primaryState));
			currStateText.text = primaryState + numTargetsChosen;
			currStateBonusText.text = secondaryState;
			currStateBonusObj.SetActive(!string.IsNullOrWhiteSpace(secondaryState));
		}

		public void UpdateCurrState(string primaryState = null, string secondaryState = null, string numTargetsChosen = null)
			=> SetCurrState(primaryState ?? this.primaryState, secondaryState ?? this.secondaryState, numTargetsChosen ?? string.Empty);

		public void AwaitingResponse(bool friendlyAlreadyAnswered)
			=> UpdateCurrState(friendlyAlreadyAnswered ? AwaitingEnemyResponse : AwaitingResponseMessage);

		public void AwaitingChoice(bool friendlyChoice, string effBlurb)
			=> UpdateCurrState(friendlyChoice ? AwaitingYourChoice : AwaitingEnemyChoice, effBlurb);

		public void ActivatingEffect(string cardName, string effBlurb)
			=> UpdateCurrState($"Effect of {cardName} activated", effBlurb);

		public void ResolvingEffect(string cardName, string effBlurb)
			=> UpdateCurrState($"Effect of {cardName} resolves", effBlurb);

		public void ChooseSpaceTarget(string cardName, string targetBlurb)
			=> SetCurrState($"Choose {cardName}'s Space Target", targetBlurb);

		public void ChooseCardTarget(string cardName, string targetBlurb)
			=> SetCurrState($"Choose {cardName}'s Card Target", targetBlurb);

		public void TargetAccepted() => SetCurrState("Target Accepted");

		public void ShuffleToHandSize() => SetCurrState("Reshuffle Down to Hand Size");

		public void EffectImpossible() => SetCurrState("Effect Impossible");

		public void NothingHappening() => SetCurrState("Nothing Happening");

		public void AttackStarted(string attackerName, string defenderName)
			=> SetCurrState("Attack Started", $"{attackerName} attacks {defenderName}");
	}
}