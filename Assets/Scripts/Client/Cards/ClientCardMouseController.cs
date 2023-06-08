using KompasClient.GameCore;
using KompasClient.UI;
using KompasCore.Cards;
using KompasCore.GameCore;
using KompasCore.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KompasClient.Cards
{
	//[RequireComponent(typeof(ClientCardController))]
	public class ClientCardMouseController : CardMouseController
	{
		private const int RightMouseButton = 1;

		public ClientCardController clientCardController;
		public ClientUIController ClientUIController => clientCardController.ClientUIController;
		public override UIController UIController => ClientUIController;

		public override void OnMouseExit()
		{
			base.OnMouseExit();
			ClientUIController.cardInfoViewUIController.Refresh();
		}

		public override void OnMouseOver()
		{
			base.OnMouseOver();

			if (Input.GetMouseButtonDown(RightMouseButton)) ClientUIController.rightClickUIController.Show(card.Card);
		}

		public override void OnMouseUp()
		{
			//don't do anything if we're over an event system object, 
			//because that would let us click on cards underneath prompts
			if (OverActualUIElement)
			{
				Debug.Log($"Released card while pointer over event system object {EventSystem.current.currentSelectedGameObject.name}");
				return;
			}

			Debug.Log($"Clicked {card} while target mode is {ClientUIController.TargetMode}");
			//don't allow dragging cards if we're awaiting a target
			if (ClientUIController.TargetMode != TargetMode.Free && ClientUIController.CardViewController.FocusedCard == card.Card)
			{
				ClientUIController.clientGame.searchCtrl.ToggleTarget(clientCardController.Card);
			}

			base.OnMouseUp();
		}
	}
}