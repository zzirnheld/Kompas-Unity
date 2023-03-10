using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace KompasDeckbuilder.UI.Deck
{
    /// <summary>
    /// Controls the middle third of the Deckbuilding UI, which handles the actual deck being built.
    /// </summary>
    public class DeckPaneController : MonoBehaviour
    {
        public static string DeckFilesFolderPath { get; private set; }
        
        public DeckPaneDeckController deckController;
        public DeckPaneDropdownController dropdownController;
        public DeckPaneSaveController saveController;

        public GameObject moreDeckButtonsParent;

        public void Start()
        {
            DeckFilesFolderPath ??= Path.Combine(Application.persistentDataPath, "Decks");
            
            dropdownController.Load();
        }

        public void ShowMoreButtons(bool show)
        {
            moreDeckButtonsParent.SetActive(show);
        }

        public void CreateDeck(string deckName, IList<string> deckList, bool save = false)
        {
            deckController.SetDecklist(deckName, deckList);
            int index = dropdownController.AddDeckListToDropdown(deckName, deckList);
            dropdownController.Select(index);
            if (save) saveController.SaveDeck();
        }

    }
}