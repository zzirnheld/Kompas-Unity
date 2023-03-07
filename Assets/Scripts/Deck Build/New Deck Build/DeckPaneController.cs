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


        public void Awake()
        {
            DeckFilesFolderPath ??= Path.Combine(Application.persistentDataPath, "Decks");
            
            dropdownController.Load();
        }

    }
}