using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

namespace KompasDeckbuilder.UI.Deck
{
    public class DeckPaneDeleteController : MonoBehaviour
    {
        public DeckPaneDeckController deckController;
        public DeckPaneDropdownController dropdownController;

        public void Delete()
        {
            var deckName = deckController.CurrDeckName;
            deckController.Delete(deckName);
            dropdownController.RemoveFromDropdown(deckName);
        }
    }
}