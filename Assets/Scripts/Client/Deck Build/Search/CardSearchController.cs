using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using KompasCore.Cards;
using System;

namespace KompasDeckbuilder
{
    public class CardSearchController : MonoBehaviour
    {
        public const string cardBackPath = "Detailed Sprites/Square Kompas Logo";

        public GameObject deckSearchInfoObj;

        //card data pane ui elements
        public GameObject CardSearchPaneParentObj;
        public Image CardImage;
        public TMP_Text CardNameText;
        public TMP_Text StatsText;
        public TMP_Text SubtypesText;
        public TMP_Text EffectText;
        private Sprite CardBack;

        //card prefabs
        public GameObject CharPrefab;
        public GameObject SpellPrefab;
        public GameObject AugPrefab;

        //other scripts
        public DeckbuilderController DeckbuilderCtrl;
        public CardRepository CardRepo;

        //search data
        private DeckbuilderCard selectedCard;
        private List<DeckSearchInfoController> shownCards;
        private string cardNameToSearch = "";
        private string subtypeToSearch = "";
        private string textToSearch = "";

        public void Awake()
        {
            //initialize list
            shownCards = new List<DeckSearchInfoController>();

            //get image to show when no card is selected
            CardBack = Resources.Load<Sprite>(cardBackPath);
            //show the blank card
            ShowSelectedCard();
        }

        /// <summary>
        /// Show the card currently selected instead of another one.
        /// </summary>
        public void ShowSelectedCard()
        {
            //if there is a selected card, show it
            if (selectedCard != null) selectedCard.Show();
            //otherwise, show data for no card, and show the card back as the sprite
            else
            {
                CardImage.sprite = CardBack;
                CardNameText.text = "";
                StatsText.text = "";
                SubtypesText.text = "";
                EffectText.text = "";
            }
        }

        public void Select(DeckbuilderCard card)
        {
            selectedCard = card;
            card.Show();
        }

        private void SearchCards()
        {
            //first clear all shown cards
            foreach (var card in shownCards)
            {
                card.Kill();
                Destroy(card.gameObject);
            }
            shownCards.Clear();

            //don't do anything if it's an invalid string to search with
            if ((string.IsNullOrWhiteSpace(cardNameToSearch) || cardNameToSearch.Length < 2)
                && (string.IsNullOrWhiteSpace(subtypeToSearch) || subtypeToSearch.Length < 2)
                && (string.IsNullOrWhiteSpace(textToSearch) || textToSearch.Length < 2))
                return;

            string nameLower = cardNameToSearch?.ToLower();
            string subtypesLower = subtypeToSearch?.ToLower();
            string textLower = textToSearch?.ToLower();

            var serializeds = CardRepository.SerializableCards;

            if (!string.IsNullOrWhiteSpace(nameLower) && nameLower.Length >= 2)
                serializeds = serializeds.Where(s => s.cardName.ToLower().Contains(nameLower));

            if (!string.IsNullOrWhiteSpace(subtypesLower) && subtypesLower.Length >= 2)
                serializeds = serializeds.Where(s => s.subtypeText.ToLower().Contains(subtypesLower));

            if (!string.IsNullOrWhiteSpace(textLower) && textLower.Length >= 2)
                serializeds = serializeds.Where(s => s.effText.ToLower().Contains(textLower));


            var jsonsThatFit = CardRepo.GetJsonsFromNames(serializeds.Select(s => s.cardName));

            //for each of the jsons, add it to the shown cards to be added
            foreach (string json in jsonsThatFit)
            {
                DeckbuilderCard newCard = CardRepo.InstantiateDeckbuilderCard(json, this, false);
                if (newCard != null)
                {
                    var cardInfo = Instantiate(deckSearchInfoObj, CardSearchPaneParentObj.transform).GetComponent<DeckSearchInfoController>();
                    cardInfo.Initialize(newCard, this);
                    shownCards.Add(cardInfo);
                }
            }
        }

        /// <summary>
        /// Called when the name input field has its data changed
        /// </summary>
        /// <param name="name">The string that should be in the name</param>
        public void SearchCardsByName(string name)
        {
            cardNameToSearch = name.Replace("\u200B", "");
            SearchCards();
        }

        /// <summary>
        /// Called when the subtype input field has its data changed
        /// </summary>
        /// <param name="subtype">The string that should be in the subtype</param>
        public void SearchCardsBySubtype(string subtype)
        {
            subtypeToSearch = subtype.Replace("\u200B", "");
            SearchCards();
        }

        public void SearchCardsByText(string text)
        {
            textToSearch = text.Replace("\u200B", "");
            SearchCards();
        }
    }
}