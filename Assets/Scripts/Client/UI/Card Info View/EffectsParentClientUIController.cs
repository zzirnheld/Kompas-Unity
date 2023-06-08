using KompasClient.GameCore;
using KompasCore.Cards;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasClient.UI
{
	public class EffectsParentClientUIController : MonoBehaviour
	{
		//public GameObject effButtonsParentObject; //should just be this.gameObject now
		public ClientSidebarCardViewController mainCardViewController;

		public Transform effBtnsParent;
		public GameObject effBtnPrefab;

		private ClientGame ClientGame => mainCardViewController.clientGame;

		private readonly List<ClientUseEffectButtonController> effBtns
			= new List<ClientUseEffectButtonController>();

		public void ShowEffButtons(GameCard card)
		{
			var effsArray = card.Effects.Where(e => e.CanBeActivatedBy(ClientGame.FriendlyPlayer)).ToArray();
			gameObject.SetActive(effsArray.Any());

			//clear existing effects
			foreach (var eff in effBtns) Destroy(eff.gameObject);
			effBtns.Clear();

			//make buttons for new effs
			foreach (var eff in effsArray)
			{
				var obj = Instantiate(effBtnPrefab, effBtnsParent);
				var ctrl = obj.GetComponent<ClientUseEffectButtonController>();
				ctrl.Initialize(eff, ClientGame.clientUIController);
				effBtns.Add(ctrl);
			}
		}
	}
}