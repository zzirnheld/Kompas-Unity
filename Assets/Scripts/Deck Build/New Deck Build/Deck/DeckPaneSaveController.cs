using System.IO;
using System.Linq;
using UnityEngine;

namespace KompasDeckbuilder.UI.Deck
{
    public class DeckPaneSaveController : MonoBehaviour
    {
        public DeckPaneDeckController deckController;

        public void SaveDeckAs(string name)
        {
            //write to a persistent file
            string filePath = Path.Combine(DeckPaneController.DeckFilesFolderPath, $"{name}.txt");

            string decklist = string.Join("\n", deckController.CurrDeckList);
            Debug.Log($"Saving deck to {filePath}:\n{decklist}");
            File.WriteAllText(filePath, decklist);

            deckController.NewDeck(name, deckController.CurrDeckList.ToArray());
        }

        public void SaveDeck() => SaveDeckAs(deckController.CurrDeckName);
    }
}