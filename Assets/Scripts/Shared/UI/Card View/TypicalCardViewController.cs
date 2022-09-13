using KompasClient.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KompasCore.UI
{
    public abstract class TypicalCardViewController : BaseCardViewController
    {
        [Header("Stats text boxes")]
        public TMP_Text nText;
        public TMP_Text eText;
        public TMP_Text costText;
        public TMP_Text wText;

        [Header("Rules text boxes")]
        public TMP_Text nameText;
        public TMP_Text subtypesText;
        public TMP_Text effText;

        [Header("Card face image")]
        public Image cardImageImage;

        public abstract CardViewReminderTextParentController ReminderTextsUIController { get; }

        protected virtual Camera Camera => Camera.main;

        protected virtual string EffTextToDisplay
        {
            get
            {
                string effText = ShownCard?.EffText;
                foreach (string keyword in CardRepository.Keywords)
                {
                    effText = effText.Replace(keyword, $"<link=\"{keyword}\"><b>{keyword}</b></link>");
                }
                return effText;
            }
        }

        private void Update()
        {
            DisplayReminderTextBlurb();
        }

        private void DisplayReminderTextBlurb()
        {
            if (ReminderTextsUIController != null)
            {
                //check keywords
                int link = TMP_TextUtilities.FindIntersectingLink(effText, Input.mousePosition, Camera);
                List<string> reminders = new List<string>();
                if (link != -1)
                {
                    var linkInfo = effText.textInfo.linkInfo[link];
                    var reminderText = CardRepository.Reminders.KeywordToReminder[linkInfo.GetLinkID()];
                    //Debug.Log($"Hovering over {linkInfo.GetLinkID()} with reminder {reminderText}");
                    reminders.Add(reminderText);
                }
                ReminderTextsUIController.Show(reminders);
                ReminderTextsUIController.transform.position = Input.mousePosition;
            }
        }


        protected override void DisplayCardRulesText()
        {
            nameText.text = ShownCard.CardName;
            subtypesText.text = ShownCard.QualifiedSubtypeText;
            effText.text = EffTextToDisplay;
        }

        protected override void DisplayCardNumericStats()
        {
            nText.text = $"N\n{ShownCard.N}";
            eText.text = $"E\n{ShownCard.E}";
            wText.text = $"W\n{ShownCard.W}";

            switch (ShownCard.CardType)
            {
                case 'C':
                    costText.text = $"S\n{ShownCard.S}";

                    nText.gameObject.SetActive(true);
                    eText.gameObject.SetActive(true);
                    wText.gameObject.SetActive(true);
                    break;
                case 'S':
                    costText.text = $"C\n{ShownCard.C}";

                    nText.gameObject.SetActive(false);
                    eText.gameObject.SetActive(false);
                    wText.gameObject.SetActive(false);
                    break;
                case 'A':
                    costText.text = $"A\n{ShownCard.A}";

                    nText.gameObject.SetActive(false);
                    eText.gameObject.SetActive(false);
                    wText.gameObject.SetActive(false);
                    break;
                default: throw new System.ArgumentException("Failed to account for new card type in displaying card's numeric stats");
            }
        }

        protected override void DisplayCardImage()
        {
            string cardFileName = ShownCard.FileName;
            var cardImageSprite = Resources.Load<Sprite>($"Simple Sprites/{cardFileName}");
            cardImageImage.sprite = cardImageSprite;
        }
    }
}