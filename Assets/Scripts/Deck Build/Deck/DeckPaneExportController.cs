using TMPro;
using UnityEngine;

namespace KompasDeckbuilder.UI.Deck
{
	public class DeckPaneExportController : MonoBehaviour
	{
		public DeckPaneController deckPaneController;
		public TMP_InputField deckListInput;

		public void Hide()
		{
			deckListInput.text = string.Empty;
			deckPaneController.Show(DeckPaneController.EditMode.Edit);
		}

		public void Show(string deckList)
		{
			deckPaneController.Show(DeckPaneController.EditMode.Export);
			deckListInput.text = deckList;
		}

		public void Show()
		{
			Show(string.Join("\n", deckPaneController.deckController.CurrDeckList));
		}
	}
}