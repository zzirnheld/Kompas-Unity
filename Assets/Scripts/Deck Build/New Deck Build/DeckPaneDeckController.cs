using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace KompasDeckbuilder.UI.Deck
{
    /// <summary>
    /// Controls the segment of the deck building UI that is the actual deck itself
    /// </summary>
    public class DeckPaneDeckController : MonoBehaviour
    {
        public Transform deckParent;

        private IDictionary<string, IList<string>> deckNameToDeckList = new Dictionary<string, IList<string>>();

        private List<DeckbuilderCard> currDeck;
        private string currDeckName;

        public IList<string> Load(string deckName)
        {
            var list = new List<string>();

            string filePath = Path.Combine(DeckPaneController.DeckFilesFolderPath, $"{deckName}.txt");
            string decklist = File.ReadAllText(filePath);

            //These have to be string replaces so they can replace with nothing
            decklist = decklist.Replace("\u200B", "");
            decklist = decklist.Replace("\r", "");
            decklist = decklist.Replace("\t", "");
            var cardNames = new List<string>(decklist.Split('\n'));
            deckNameToDeckList[deckName] = cardNames;

            return cardNames;
        }

        public void Show(string deckName)
        {
            if (currDeckName != default) ; //TODO clear old deck

            
        }
    }
}