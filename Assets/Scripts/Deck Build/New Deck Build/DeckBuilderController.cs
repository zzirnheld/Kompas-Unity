using KompasDeckbuilder.UI.Deck;
using UnityEngine;

namespace KompasDeckbuilder.UI
{
    public class DeckBuilderController : MonoBehaviour
    {
        public CardRepository cardRepo;

        public DeckPaneController deckPaneController;
        public DeckbuilderCardViewController cardViewController;

        public DeckBuilderCardController CurrentDrag { get; set; }

        public void DragEnteredIndex(int index)
        {
            if (null == CurrentDrag) return;

            CurrentDrag.transform.SetSiblingIndex(index);
            deckPaneController.deckController.ChangeDeckIndex(CurrentDrag, index);
        }

    }
}