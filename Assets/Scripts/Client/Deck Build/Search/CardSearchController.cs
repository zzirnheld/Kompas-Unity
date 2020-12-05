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
        public const char nbsp = (char)8203;

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

        public TMP_Text NameToSearch;
        public TMP_Text SubtypeToSearch;
        public TMP_Text TextToSearch;

        public Toggle CharacterToggle;
        public Toggle SpellToggle;
        public Toggle AugmentToggle;

        public GameObject MoreSearchOptions;

        public TMP_Text NMin;
        public TMP_Text EMin;
        public TMP_Text SMin;
        public TMP_Text WMin;
        public TMP_Text CMin;
        public TMP_Text AMin;

        public TMP_Text NMax;
        public TMP_Text EMax;
        public TMP_Text SMax;
        public TMP_Text WMax;
        public TMP_Text CMax;
        public TMP_Text AMax;

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

        private bool ValidCardType(char cardType)
        {
            switch (cardType)
            {
                case 'C': return CharacterToggle.isOn;
                case 'S': return SpellToggle.isOn;
                case 'A': return AugmentToggle.isOn;
                default: return false;
            }
        }

        public void SearchCards()
        {
            //first clear all shown cards
            foreach (var card in shownCards)
            {
                card.Kill();
                Destroy(card.gameObject);
            }
            shownCards.Clear();

            cardNameToSearch = NameToSearch.text.Replace("\u200B", "");
            subtypeToSearch = SubtypeToSearch.text.Replace("\u200B", "");
            textToSearch = TextToSearch.text.Replace("\u200B", "");

            //don't do anything if it's an invalid string to search with
            if ((string.IsNullOrWhiteSpace(cardNameToSearch) || cardNameToSearch.Length < 2)
                && (string.IsNullOrWhiteSpace(subtypeToSearch) || subtypeToSearch.Length < 2)
                && (string.IsNullOrWhiteSpace(textToSearch) || textToSearch.Length < 2)
                && string.IsNullOrWhiteSpace(NMin.text)
                && string.IsNullOrWhiteSpace(EMin.text)
                && string.IsNullOrWhiteSpace(SMin.text)
                && string.IsNullOrWhiteSpace(WMin.text)
                && string.IsNullOrWhiteSpace(CMin.text)
                && string.IsNullOrWhiteSpace(AMin.text)
                && string.IsNullOrWhiteSpace(NMax.text)
                && string.IsNullOrWhiteSpace(EMax.text)
                && string.IsNullOrWhiteSpace(SMax.text)
                && string.IsNullOrWhiteSpace(WMax.text)
                && string.IsNullOrWhiteSpace(CMax.text)
                && string.IsNullOrWhiteSpace(AMax.text))
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

            if (int.TryParse(NMin.text.Trim(nbsp), out int nMin)) serializeds = serializeds.Where(s => s.n >= nMin);
            if (int.TryParse(EMin.text.Trim(nbsp), out int eMin)) serializeds = serializeds.Where(s => s.e >= eMin);
            if (int.TryParse(SMin.text.Trim(nbsp), out int sMin)) serializeds = serializeds.Where(s => s.s >= sMin);
            if (int.TryParse(WMin.text.Trim(nbsp), out int wMin)) serializeds = serializeds.Where(s => s.w >= wMin);
            if (int.TryParse(CMin.text.Trim(nbsp), out int cMin)) serializeds = serializeds.Where(s => s.c >= cMin);
            if (int.TryParse(AMin.text.Trim(nbsp), out int aMin)) serializeds = serializeds.Where(s => s.a >= aMin);

            if (int.TryParse(NMax.text.Trim(nbsp), out int nMax)) serializeds = serializeds.Where(s => s.n <= nMax);
            if (int.TryParse(EMax.text.Trim(nbsp), out int eMax)) serializeds = serializeds.Where(s => s.e <= eMax);
            if (int.TryParse(SMax.text.Trim(nbsp), out int sMax)) serializeds = serializeds.Where(s => s.s <= sMax);
            if (int.TryParse(WMax.text.Trim(nbsp), out int wMax)) serializeds = serializeds.Where(s => s.w <= wMax);
            if (int.TryParse(CMax.text.Trim(nbsp), out int cMax)) serializeds = serializeds.Where(s => s.c <= cMax);
            if (int.TryParse(AMax.text.Trim(nbsp), out int aMax)) serializeds = serializeds.Where(s => s.a <= aMax);

            serializeds = serializeds.Where(s => ValidCardType(s.cardType));

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

        public void ShowMoreSearchOption(bool show) => MoreSearchOptions.SetActive(show);
    }
}