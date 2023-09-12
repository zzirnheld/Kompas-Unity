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

		public ClientGame clientGame;
		public TargetMode TargetMode { get; set; } = TargetMode.Free;

		public override IReminderTextParentController ReminderTextParentUIController => cardInfoViewUIController.ReminderTextsUIController;

		public ConnectionUIController connectionUIController;
		public CurrentStateUIController currentStateUIController;
		public EffectsUIController effectsUIController;

		[Header("Card Materials")]
		public Material friendlyCardFrameMaterial;
		public Material enemyCardFrameMaterial;

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
			Debug.Log($"Friendly {clientSettings.FriendlyColor}");
			friendlyCardFrameMaterial.color = clientSettings.FriendlyColor;
			enemyCardFrameMaterial.color = clientSettings.EnemyColor;
		}
			//TODO if (fromClick && targetMode != Game.TargetMode.Free && card != null) clientGame.searchCtrl.ToggleTarget(card);

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