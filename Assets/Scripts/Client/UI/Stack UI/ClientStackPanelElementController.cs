using TMPro;
using UnityEngine;
using UnityEngine.UI;
using KompasCore.Cards;
using KompasClient.Effects;
using UnityEngine.EventSystems;

namespace KompasClient.UI
{
	public class ClientStackPanelElementController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		public Image primaryImage;
		public Image secondaryImage;
		public TMP_Text blurbText;

		private IClientStackable stackable;
		private CardController primaryCardCtrl;
		private CardController secondaryCardCtrl;

		public void Initialize(IClientStackable stackable)
		{
			var primary = stackable.PrimarySprite;
			if (primary != null) primaryImage.sprite = primary;
			primaryImage.gameObject.SetActive(primary != null);

			var secondary = stackable.SecondarySprite;
			if (secondary != null) secondaryImage.sprite = secondary;
			secondaryImage.gameObject.SetActive(secondary != null);

			blurbText.text = stackable.StackableBlurb;

			this.stackable = stackable;
			primaryCardCtrl = stackable.PrimaryCardController;
			secondaryCardCtrl = stackable.SecondaryCardController;
		}

		private void ShowStackableRoles(bool show)
		{
			if (primaryCardCtrl != null) primaryCardCtrl.gameCardViewController.ShowPrimaryOfStackable(show);
			if (secondaryCardCtrl != null) secondaryCardCtrl.gameCardViewController.ShowSecondaryOfStackable(show);
		}

		public void OnPointerEnter(PointerEventData eventData) => ShowStackableRoles(true);

		public void OnPointerExit(PointerEventData eventData) => ShowStackableRoles(false);

		public void OnDestroy() => ShowStackableRoles(false);
	}
}