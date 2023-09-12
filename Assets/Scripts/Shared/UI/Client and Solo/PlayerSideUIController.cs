using KompasClient.GameCore;
using KompasCore.GameCore;
using KompasCore.UI;
using UnityEngine;

namespace KompasClient.UI
{
	public abstract class PlayerSideUIController : UIController
	{
		public ClientGame clientGame;

		[Header("UI Controllers")]
		public ConnectionUIController connectionUIController;
		public CurrentStateUIController currentStateUIController;
		public EffectsUIController effectsUIController;
		[Tooltip("Effect activation")]
		public RightClickCardClientUIController rightClickUIController;
		public EscapeMenuUIController escapeMenuUIController;
		public ClientSidebarCardViewController cardInfoViewUIController;
		public override SidebarCardViewController CardViewController => cardInfoViewUIController;
		public override IReminderTextParentController ReminderTextParentUIController => cardInfoViewUIController.ReminderTextsUIController;
		public ClientBoardUIController boardUIController;


		[Header("Card Materials")]
		public Material friendlyCardFrameMaterial;
		public Material enemyCardFrameMaterial;

		public TargetMode TargetMode { get; set; } = TargetMode.Free;

		private void Update()
		{
			//when the user presses escape, show the menu.
			if (Input.GetKeyDown(KeyCode.Escape)) escapeMenuUIController.Enable();
		}

		public virtual void ApplySettings(ClientSettings clientSettings)
		{
			friendlyCardFrameMaterial.color = clientSettings.FriendlyColor;
			enemyCardFrameMaterial.color = clientSettings.EnemyColor;
		}
	}
}