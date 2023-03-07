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
        public DeckBuilderController deckBuilderController;
        public Transform deckParent;

        private CardRepository CardRepo => deckBuilderController.cardRepo;

        private IDictionary<string, IList<string>> deckNameToDeckList = new Dictionary<string, IList<string>>();

        private List<DeckbuilderCardController> currDeck;
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
            if (currDeckName == deckName) return;
            if (currDeckName != default) ; //TODO clear old deck

            currDeckName = deckName;
            var cardNames = deckNameToDeckList[currDeckName];

            foreach (string name in cardNames)
            {
                if (!string.IsNullOrWhiteSpace(name)) AddToDeck(name);
            }
        }

        public void AddToDeck(string name)
        {
            string json = CardRepo.GetJsonFromName(name);
            if (json == null) return;

            //TODO refactor out whatever CardSearchController is doing there with the selecting thing
            DeckbuilderCardController toAdd = CardRepo.InstantiateDeckbuilderCard(json, default, true);
            if (toAdd == null)
            {
                Debug.LogError($"Somehow have a DeckbuilderCard with name {name} couldn't be re-instantiated");
                return;
            }

            currDeck.Add(toAdd);
            toAdd.gameObject.SetActive(true);
            toAdd.transform.SetParent(deckParent);
            toAdd.transform.localScale = Vector3.one;
        }
    }
}