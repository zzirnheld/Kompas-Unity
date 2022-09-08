using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using KompasCore.Cards;
using UnityEngine.EventSystems;
using System.Linq;
using KompasClient.GameCore;
using Newtonsoft.Json;

namespace KompasClient.UI
{
    public class CardInfoViewClientUIController : MonoBehaviour, IPointerExitHandler
    {
        public const string RemindersJsonPath = "Reminder Text/Reminder Texts";

        public Sprite charHaze;
        public Sprite nonCharHaze;
        public Sprite charFrame;
        public Sprite nonCharFrame;

        public GameObject conditionParentObject;
        public GameObject negatedObject;
        public GameObject activatedObject;


        public ClientGame clientGame;

        private readonly List<ReminderTextClientUIController> reminderCtrls
            = new List<ReminderTextClientUIController>();
        public Transform remindersParent;
        public GameObject reminderPrefab;

        public ClientSearchUIController searchUICtrl;

        private readonly List<GameCard> shownUniqueCopies = new List<GameCard>();
        private readonly HashSet<GameCard> shownLinkedCards = new HashSet<GameCard>();

        public ReminderTextsContainer Reminders { get; private set; }

        /// <summary>
        /// The currently shown card by this card info view control controller.
        /// Setting this also takes care of showing the relevant info.
        /// </summary>
        public GameCard CurrShown { get; private set; }

        public void Awake()
        {
            var jsonAsset = Resources.Load<TextAsset>(RemindersJsonPath);
            Reminders = JsonConvert.DeserializeObject<ReminderTextsContainer>(jsonAsset.text);
            gameObject.SetActive(false);
        }

        private void ShowReminderText()
        {
            //clear existing reminders
            foreach (var reminderCtrl in reminderCtrls) Destroy(reminderCtrl.gameObject);
            reminderCtrls.Clear();
            //create new reminders
            foreach (var reminder in Reminders.keywordReminderTexts)
            {
                if (CurrShown.EffText.Contains(reminder.keyword))
                {
                    var obj = Instantiate(reminderPrefab, remindersParent);
                    var ctrl = obj.GetComponent<ReminderTextClientUIController>();
                    ctrl.Initialize(reminder.keyword, reminder.reminder);
                    reminderCtrls.Add(ctrl);
                }
            }
            remindersParent.gameObject.SetActive(reminderCtrls.Any());
        }

        private void ClearShownUniqueCopies()
        {
            foreach (var c in shownUniqueCopies) c.cardCtrl.ShowUniqueCopy(false);
            shownUniqueCopies.Clear();
        }

        private void ShowUniqueCopies()
        {
            ClearShownUniqueCopies();
            if (CurrShown.Unique)
            {
                //deal with unique cards
                var copies = clientGame.Cards.Where(c => c.Location == CardLocation.Board && c.IsFriendlyCopyOf(CurrShown));
                foreach (var copy in copies)
                {
                    copy.cardCtrl.ShowUniqueCopy(true);
                    shownUniqueCopies.Add(copy);
                }
            }
        }

        private void ClearShownCardLinks()
        {
            foreach (var c in shownLinkedCards) c.cardCtrl.ShowLinkedCard(false);
            shownLinkedCards.Clear();
        }

        private void ShowCardLinks()
        {
            ClearShownCardLinks();
            foreach (var link in CurrShown.CardLinkHandler.Links)
            {
                foreach(var card in link.CardIDs.Select(clientGame.GetCardWithID))
                {
                    if (card == default) continue;
                    shownLinkedCards.Add(card);
                    card.cardCtrl.ShowLinkedCard(true);
                }
            }
        }

        public void ShowForCurrShown()
        {
            //null means show nothing
            if (CurrShown == null)
            {
                ClearSpecialShownInfo();
                gameObject.SetActive(false);
                return;
            }

            conditionParentObject.SetActive(CurrShown.Negated || CurrShown.Activated);
            negatedObject.SetActive(CurrShown.Negated);
            activatedObject.SetActive(CurrShown.Activated);

            ShowEffButtons();
            ShowReminderText();

            searchUICtrl.HideIfNotShowingCurrSearchIndex();

            ShowUniqueCopies();
            ShowCardLinks();
            ShowPipsAvailableForCost();

            gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (searchUICtrl.Searching) searchUICtrl.ReshowSearchShown();
            else CurrShown = null;
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt))
                CurrShown = null;
        }
    }
}