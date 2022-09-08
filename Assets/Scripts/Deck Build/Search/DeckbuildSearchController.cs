using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using KompasCore.Cards;
using KompasClient.UI;
using Newtonsoft.Json;

namespace KompasDeckbuilder
{
    public class DeckbuildSearchController : MonoBehaviour
    {
        public const string CardBackPath = "Detailed Sprites/Square Kompas Logo";
        public const string RemindersJsonPath = "Reminder Text/Reminder Texts";
        public const char NBSP = (char)8203;
        public const int MaxToShow = 100;

        public GameObject deckSearchInfoObj;

        //card data pane ui elements
        public GameObject CardSearchPaneParentObj;

        public GameObject cardViewParentObj;
        public Image CardImage;
        public TMP_Text CardNameText;
        public TMP_Text nText;
        public TMP_Text eText;
        public TMP_Text scaText;
        public TMP_Text wText;
        public TMP_Text SubtypesText;
        public TMP_Text EffectText;

        private Sprite CardBack;
        private readonly List<ReminderTextUIController> reminderCtrls
            = new List<ReminderTextUIController>();
        public Transform remindersParent;
        public GameObject reminderPrefab;
        public ReminderTextsContainer Reminders { get; private set; }

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
            CardBack = Resources.Load<Sprite>(CardBackPath);
            //show the blank card
            ShowSelectedCard();

            var jsonAsset = Resources.Load<TextAsset>(RemindersJsonPath);
            Reminders = JsonConvert.DeserializeObject<ReminderTextsContainer>(jsonAsset.text);
        }

        /// <summary>
        /// Show the card currently selected instead of another one.
        /// </summary>
        public void ShowSelectedCard()
        {
            cardViewParentObj.SetActive(selectedCard != null);
            //if there is a selected card, show it
            if (selectedCard != null) selectedCard.Show();
            //otherwise, show data for no card, and show the card back as the sprite
            else
            {
                CardImage.sprite = CardBack;
                CardNameText.text = "";
                SubtypesText.text = "";
                EffectText.text = "";
            }

            ShowReminderText(selectedCard);
        }


        public void ShowReminderText(CardBase cardInfo)
        {
            //clear existing reminders
            foreach (var reminderCtrl in reminderCtrls) Destroy(reminderCtrl.gameObject);
            reminderCtrls.Clear();
            if (cardInfo == null)
            {
                remindersParent.gameObject.SetActive(false);
                return;
            }
            //create new reminders
            foreach (var reminder in Reminders.keywordReminderTexts)
            {
                if (cardInfo.EffText.Contains(reminder.keyword))
                {
                    var obj = Instantiate(reminderPrefab, remindersParent);
                    var ctrl = obj.GetComponent<ReminderTextUIController>();
                    ctrl.Initialize(reminder.keyword, reminder.reminder);
                    reminderCtrls.Add(ctrl);
                }
            }
            remindersParent.gameObject.SetActive(reminderCtrls.Any());
        }

        public void Select(DeckbuilderCard card)
        {
            selectedCard = card;
            cardViewParentObj.SetActive(true);
            card.Show();
        }

        private bool ValidCardType(char cardType)
        {
            return cardType switch
            {
                'C' => CharacterToggle.isOn,
                'S' => SpellToggle.isOn,
                'A' => AugmentToggle.isOn,
                _ => false,
            };
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

            //assume that name/subtype/text to search have already been set.
            //setting them right now, if this is called as an event for on edit,
            //will cause the value currently in .text to be the value **before** the edit.

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

            if (!string.IsNullOrWhiteSpace(nameLower))
                serializeds = serializeds.Where(s => s.cardName.ToLower().Contains(nameLower));

            if (!string.IsNullOrWhiteSpace(subtypesLower))
                serializeds = serializeds.Where(s => s.subtypeText.ToLower().Contains(subtypesLower));

            if (!string.IsNullOrWhiteSpace(textLower))
                serializeds = serializeds.Where(s => s.effText.ToLower().Contains(textLower));

            if (int.TryParse(NMin.text.Trim(NBSP), out int nMin)) serializeds = serializeds.Where(s => s.n >= nMin);
            if (int.TryParse(EMin.text.Trim(NBSP), out int eMin)) serializeds = serializeds.Where(s => s.e >= eMin);
            if (int.TryParse(SMin.text.Trim(NBSP), out int sMin)) serializeds = serializeds.Where(s => s.s >= sMin);
            if (int.TryParse(WMin.text.Trim(NBSP), out int wMin)) serializeds = serializeds.Where(s => s.w >= wMin);
            if (int.TryParse(CMin.text.Trim(NBSP), out int cMin)) serializeds = serializeds.Where(s => s.c >= cMin);
            if (int.TryParse(AMin.text.Trim(NBSP), out int aMin)) serializeds = serializeds.Where(s => s.a >= aMin);

            if (int.TryParse(NMax.text.Trim(NBSP), out int nMax)) serializeds = serializeds.Where(s => s.n <= nMax);
            if (int.TryParse(EMax.text.Trim(NBSP), out int eMax)) serializeds = serializeds.Where(s => s.e <= eMax);
            if (int.TryParse(SMax.text.Trim(NBSP), out int sMax)) serializeds = serializeds.Where(s => s.s <= sMax);
            if (int.TryParse(WMax.text.Trim(NBSP), out int wMax)) serializeds = serializeds.Where(s => s.w <= wMax);
            if (int.TryParse(CMax.text.Trim(NBSP), out int cMax)) serializeds = serializeds.Where(s => s.c <= cMax);
            if (int.TryParse(AMax.text.Trim(NBSP), out int aMax)) serializeds = serializeds.Where(s => s.a <= aMax);

            serializeds = serializeds.Where(s => ValidCardType(s.cardType));

            var serializedArray = serializeds.ToArray();

            if (serializedArray.Length > MaxToShow) return;

            var jsonsThatFit = CardRepo.GetJsonsFromNames(serializedArray.Select(s => s.cardName));

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

        public void SearchName(string name)
        {
            cardNameToSearch = name.Replace("\u200B", "");
            SearchCards();
        }

        public void SearchSubtypes(string subtypes)
        {
            subtypeToSearch = subtypes.Replace("\u200B", "");
            SearchCards();
        }

        public void SearchText(string text)
        {
            textToSearch = text.Replace("\u200B", "");
            SearchCards();
        }

        public void ShowMoreSearchOption(bool show)
        {
            MoreSearchOptions.SetActive(show);
            if (!show) SearchCards();
        }
    }
}