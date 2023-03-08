using KompasCore.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace KompasDeckbuilder.UI.Search
{
    public class DeckBuildSearchController : MonoBehaviour
    {
        private const int MaxToShow = 100;

        public DeckBuilderController deckBuilderController;

        public GameObject deckSearchCardPrefab;
        public Transform searchGridParentTransform;

        public TMP_InputField searchText;
        private string textToSearch => searchText.text;

        private IList<DeckBuilderSearchCardController> shownCards = new List<DeckBuilderSearchCardController>();

        public void SearchCards()
        {
            //first clear all shown cards
            foreach (var card in shownCards)
            {
                Destroy(card.gameObject);
            }
            shownCards.Clear();

            //assume that name/subtype/text to search have already been set.
            //setting them right now, if this is called as an event for on edit,
            //will cause the value currently in .text to be the value **before** the edit.

            //don't do anything if it's an invalid string to search with
            if (string.IsNullOrWhiteSpace(textToSearch) || textToSearch.Length < 2) return;

            string textLower = textToSearch?.ToLower();

            var serializeds = CardRepository.SerializableCards;

            if (!string.IsNullOrWhiteSpace(textLower))
                serializeds = serializeds.Where(TextIncludes(textLower));

            var serializedArray = serializeds.ToArray();

            if (serializedArray.Length > MaxToShow) return;

            var jsonsThatFit = deckBuilderController.cardRepo.GetJsonsFromNames(serializedArray.Select(s => s.cardName));

            //for each of the jsons, add it to the shown cards to be added
            foreach (string json in jsonsThatFit)
            {
                var newCard = deckBuilderController.cardRepo.InstantiateDeckBuilderCard(json, deckBuilderController);
                if (newCard != null)
                {
                    var cardInfo = Instantiate(deckSearchCardPrefab, searchGridParentTransform).GetComponent<DeckBuilderSearchCardController>();
                    cardInfo.Initialize(newCard, deckBuilderController.deckPaneController.deckController);
                    shownCards.Add(cardInfo);
                }
            }
        }

        private static Func<SerializableCard, bool> TextIncludes(string textLower)
        {
            return s => s.effText.ToLower().Contains(textLower)
                || s.cardName.ToLower().Contains(textLower)
                // || s.subt.Any(subtype => subtype.ToLower().Contains(textLower))
                || s.subtypeText.ToLower().Contains(textLower);
        }
    }
}