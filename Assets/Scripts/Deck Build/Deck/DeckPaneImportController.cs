using KompasCore.Helpers;
using TMPro;
using UnityEngine;

namespace KompasDeckbuilder.UI.Deck
{
	public class DeckPaneImportController : MonoBehaviour
	{
		public DeckPaneController deckPaneController;
		public TMP_InputField deckName;
		public TMP_InputField deckList;

		public void Hide()
		{
			deckName.text = string.Empty;
			deckList.text = string.Empty;
			deckPaneController.Show(DeckPaneController.EditMode.Edit);
		}

		public void Confirm()
		{
			Debug.Log($"Confirmed {deckName.text} as {deckList.text}");
			deckPaneController.CreateDeck(deckName.text, DeckLoadHelper.CleanAndSplit(deckList.text), save: true);
			Hide();
		}

		public void Show() => deckPaneController.Show(DeckPaneController.EditMode.Import);
	}
}