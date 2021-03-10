using System.Collections;
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

        public ReminderTextsContainer Reminders { get; private set; }

        private GameCard currShown;
        /// <summary>
        /// The currently shown card by this card info view control controller.
        /// Setting this also takes care of showing the relevant info.
        /// </summary>
        public GameCard CurrShown
        {
            get => currShown;
            set
            {
                //don't waste time updating if currently showing that.
                if (currShown == value) return;

                //save value, then
                currShown = value;

                ShowForCurrShown();
            }
        }

        public void Awake()
        {
            var jsonAsset = Resources.Load<TextAsset>(RemindersJsonPath);
            Reminders = JsonConvert.DeserializeObject<ReminderTextsContainer>(jsonAsset.text);
            gameObject.SetActive(false);
        }

        public void ShowForCurrShown()
        {
            //null means show nothing
            if (currShown == null)
            {
                gameObject.SetActive(false);
                return;
            }

            bool isChar = currShown.CardType == 'C';
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

            cardFaceImage.sprite = currShown.simpleSprite;

            nText.text = $"N\n{currShown.N}";
            eText.text = $"E\n{currShown.E}";
            wText.text = $"W\n{currShown.W}";
            nText.gameObject.SetActive(isChar);
            eText.gameObject.SetActive(isChar);
            wText.gameObject.SetActive(isChar);

            //TODO after unity updates: make this a switch expression
            switch (currShown.CardType)
            {
                case 'C': costText.text = $"S\n{currShown.S}"; break;
                case 'S': costText.text = $"C\n{currShown.C}"; break;
                case 'A': costText.text = $"A\n{currShown.A}"; break;
                default: throw new System.NotImplementedException();
            }

            nameText.text = currShown.CardName;
            subtypesText.text = currShown.QualifiedSubtypeText;
            effText.text = currShown.EffText;

            conditionParentObject.SetActive(currShown.Negated || currShown.Activated);
            negatedObject.SetActive(currShown.Negated);
            activatedObject.SetActive(currShown.Activated);

            var effsArray = currShown.Effects.Where(e => e.CanBeActivatedBy(clientGame.Players[0])).ToArray();
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

            //clear existing reminders
            foreach (var reminderCtrl in reminderCtrls) Destroy(reminderCtrl.gameObject);
            reminderCtrls.Clear();
            //create new reminders
            foreach (var reminder in Reminders.keywordReminderTexts)
            {
                if (currShown.EffText.Contains(reminder.keyword))
                {
                    var obj = Instantiate(reminderPrefab, remindersParent);
                    var ctrl = obj.GetComponent<ReminderTextClientUIController>();
                    ctrl.Initialize(reminder.keyword, reminder.reminder);
                    reminderCtrls.Add(ctrl);
                }
            }
            remindersParent.gameObject.SetActive(reminderCtrls.Any());

            searchUICtrl.HideIfNotShowingCurrSearchIndex();

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