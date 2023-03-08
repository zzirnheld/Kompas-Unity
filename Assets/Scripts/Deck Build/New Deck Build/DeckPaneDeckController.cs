using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private IList<DeckBuilderCardController> currDeck = new List<DeckBuilderCardController>();
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
            ClearDeck();

            currDeckName = deckName;
            var cardNames = deckNameToDeckList[currDeckName]
                .Where(name => !string.IsNullOrWhiteSpace(name));

            foreach (string name in cardNames) AddToDeck(name);
        }

        private void ClearDeck()
        {
            if (currDeck == null) return;
            
            foreach (var card in currDeck) Destroy(card);
            currDeck.Clear();
        }

        public void AddToDeck(string name)
        {
            string json = CardRepo.GetJsonFromName(name);
            if (json == null) return;

            var toAdd = CardRepo.InstantiateDeckBuilderCard(json, deckBuilderController);

            currDeck.Add(toAdd);
            toAdd.gameObject.SetActive(true);
            toAdd.transform.SetParent(deckParent);
            toAdd.transform.localScale = Vector3.one;
        }

        public void RemoveFromDeck(DeckBuilderCardController card)
        {
            if (!currDeck.Remove(card)) return;

            Destroy(card);
        }

        public void ChangeDeckIndex(DeckBuilderCardController card, int index)
        {
            if (!currDeck.Remove(card)) return;

            currDeck.Insert(index, card);

            deckNameToDeckList[currDeckName] = currDeck.Select(card => card.CardName).ToArray();
        }
    }
}