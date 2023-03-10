using TMPro;
using UnityEngine;

namespace KompasDeckbuilder.UI.Deck
{
    public class DeckPaneImportController : MonoBehaviour
    {
        public DeckPaneController deckPaneController;
        public GameObject deckEditParent;

        public TMP_InputField deckName;
        public TMP_InputField deckList;

        public void Show(bool show)
        {
            gameObject.SetActive(show);
            deckEditParent.SetActive(!show);
        }

        public void Show() => Show(true);

        public void Hide()
        {
            deckName.text = string.Empty;
            deckList.text = string.Empty;
            Show(false);
        }

        public void Confirm()
        {
            Debug.Log($"Confirmed {deckName.text} as {deckList.text}");
            deckPaneController.CreateDeck(deckName.text, DeckPaneDeckController.Split(deckList.text), save: true);
            Hide();
        }
    }
}