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
        //public const string DefaultCharFrame = "Misc Card Icons/Character Frame";
        //public const string DefaultNonCharFrame = "Misc Card Icons/Spell Frame";
        public const string RemindersJsonPath = "Reminder Text/Reminder Texts";

        public Sprite charHaze;
        public Sprite nonCharHaze;
        public Sprite charFrame;
        public Sprite nonCharFrame;

        public TMP_Text nameText;
        public TMP_Text subtypesText;
        public TMP_Text nText;
        public TMP_Text eText;
        public TMP_Text costText;
        public TMP_Text wText;
        public TMP_Text effText;

        public Image cardFrameImage;
        public Image cardFaceImage;
        public Image cardImageHaze;

        public GameObject conditionParentObject;
        public GameObject negatedObject;
        public GameObject activatedObject;

        public GameObject effButtonsParentObject;

        public ClientGame clientGame;

        private readonly List<ClientUseEffectButtonController> effBtns 
            = new List<ClientUseEffectButtonController>();
        public Transform effBtnsParent;
        public GameObject effBtnPrefab;

        private readonly List<ReminderTextClientUIController> reminderCtrls
            = new List<ReminderTextClientUIController>();
        public Transform remindersParent;
        public GameObject reminderPrefab;

        public ClientSearchUIController searchUICtrl;

        private readonly List<GameCard> shownUniqueCopies = new List<GameCard>();

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

        public void ShowInfoFor(GameCard card, bool refresh = false)
        {
            bool reshow = card != CurrShown || refresh;
            CurrShown = card;

            if(reshow) ShowForCurrShown();
        }

        private void ShowViewForCardType()
        {
            bool isChar = CurrShown.CardType == 'C';
            if (isChar)
            {
                cardFrameImage.sprite = charFrame;
                cardImageHaze.sprite = charHaze;
            }
            else
            {
                cardFrameImage.sprite = nonCharFrame;
                cardImageHaze.sprite = nonCharHaze;
            }
            nText.gameObject.SetActive(isChar);
            eText.gameObject.SetActive(isChar);
            wText.gameObject.SetActive(isChar);
        }

        private void ShowCurrShownStats()
        {
            nText.text = $"N\n{CurrShown.N}";
            eText.text = $"E\n{CurrShown.E}";
            wText.text = $"W\n{CurrShown.W}";

            //TODO after unity updates: make this a switch expression
            costText.text = CurrShown.CardType switch
            {
                'C' => $"S\n{CurrShown.S}",
                'S' => $"C\n{CurrShown.C}",
                'A' => $"A\n{CurrShown.A}",
                _ => throw new System.NotImplementedException(),
            };
            nameText.text = CurrShown.CardName;
            subtypesText.text = CurrShown.QualifiedSubtypeText;
            effText.text = CurrShown.EffText;
        }

        private void ShowEffButtons()
        {
            var effsArray = CurrShown.Effects.Where(e => e.CanBeActivatedBy(clientGame.Players[0])).ToArray();
            effButtonsParentObject.SetActive(effsArray.Any());
            //clear existing effects
            foreach (var eff in effBtns) Destroy(eff.gameObject);
            effBtns.Clear();
            //make buttons for new effs
            foreach (var eff in effsArray)
            {
                var obj = Instantiate(effBtnPrefab, effBtnsParent);
                var ctrl = obj.GetComponent<ClientUseEffectButtonController>();
                ctrl.Initialize(eff, clientGame.clientUICtrl);
                effBtns.Add(ctrl);
            }
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

        private void ShowUniqueCopies()
        {
            if (CurrShown.Unique)
            {
                //deal with unique cards
                var copies = clientGame.Cards.Where(c => c.Location == CardLocation.Field && c.IsFriendlyCopyOf(CurrShown));
                foreach (var copy in copies)
                {
                    copy.cardCtrl.ShowUniqueCopy(true);
                    shownUniqueCopies.Add(copy);
                }
            }
        }

        public void ShowForCurrShown()
        {
            foreach (var c in shownUniqueCopies) c.cardCtrl.ShowUniqueCopy(false);
            shownUniqueCopies.Clear();

            //null means show nothing
            if (CurrShown == null)
            {
                gameObject.SetActive(false);
                return;
            }

            ShowViewForCardType();
            cardFaceImage.sprite = CurrShown.simpleSprite;

            ShowCurrShownStats();

            conditionParentObject.SetActive(CurrShown.Negated || CurrShown.Activated);
            negatedObject.SetActive(CurrShown.Negated);
            activatedObject.SetActive(CurrShown.Activated);

            ShowEffButtons();
            ShowReminderText();

            searchUICtrl.HideIfNotShowingCurrSearchIndex();

            ShowUniqueCopies();

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