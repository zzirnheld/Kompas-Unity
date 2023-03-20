using System.Collections.Generic;
using System.Text;
using KompasClient.Cards;
using KompasClient.Networking;
using KompasCore.Helpers;
using KompasCore.UI;
using TMPro;
using UnityEngine;

namespace KompasClient.UI
{
    public class NewDeckSelectUIController : DeckDropdownControllerBase
    {
        public ClientNotifier clientNotifier;
        public ClientCardRepository cardRepository;
        public DeckSelectCardController deckSelectCardPrefab;
        public Transform deckScrollAreaParentTransform;
        public TMP_Text cardsInDeckText;

        private IList<DeckSelectCardController> currDeck = new List<DeckSelectCardController>();

        public string AvatarFileName => currDeck.Count < 1 ? null : currDeck[0].Card.FileName;

        private void Start() => Load();

        public void SelectionChanged(int index) => Show(dropdown.options[index].text);

        protected override void Show(string deckName)
        {
            var cardNames = DeckLoadHelper.LoadDeck(deckName); //TODO stash, using the override of LoadDeck in DeckDropdownControllerBase

            ClearDeck();

            foreach (string name in cardNames)
            {
                if (!string.IsNullOrWhiteSpace(name)) AddToDeck(name);
            }

            SetDeckCountText();
        }

        private void SetDeckCountText()
        {
            cardsInDeckText.text = $"Cards in Deck: {currDeck.Count}";
        }

        private void ClearDeck()
        {
            foreach (var card in currDeck) Destroy(card.gameObject);
            currDeck.Clear();
        }

        private void AddToDeck(string name)
        {
            string json = cardRepository.GetJsonFromName(name);
            if (json == null)
            {
                Debug.LogError($"No json found for card name {name}");
                return;
            }

            DeckSelectCardController toAdd = cardRepository.InstantiateDeckSelectCard(json, deckScrollAreaParentTransform, deckSelectCardPrefab, null);
            if (toAdd == null)
            {
                Debug.LogError($"Somehow have a DeckbuilderCard with name {name} couldn't be re-instantiated");
                return;
            }

            currDeck.Add(toAdd);
            SetDeckCountText();
        }

        public void ConfirmSelectedDeck()
        {
            StringBuilder sb = new StringBuilder();
            foreach (DeckSelectCardController card in currDeck)
            {
                sb.Append(card.Card.CardName);
                sb.Append("\n");
            }

            clientNotifier.RequestDecklistImport(sb.ToString());
        }
    }
}