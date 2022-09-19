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

        public abstract IReminderTextParentController ReminderTextsUIController { get; }

        protected virtual Camera Camera => Camera.main;

        protected virtual string EffTextToDisplay
        {
            get
            {
                string effText = shownCard?.EffText;
                foreach (string keyword in CardRepository.Keywords)
                {
                    effText = effText.Replace(keyword, $"<link=\"{keyword}\"><b>{keyword}</b></link>");
                }
                return effText;
            }
        }

        protected virtual void Update()
        {
            DisplayReminderTextBlurb();
        }

        protected virtual void DisplayReminderTextBlurb()
        {
            if (ReminderTextsUIController != null)
            {
                //Debug.Log($"Checking keywords at {Input.mousePosition}");
                //check keywords
                int link = TMP_TextUtilities.FindIntersectingLink(effText, Input.mousePosition, Camera);
                List<(string, string)> reminders = new List<(string, string)>();
                if (link != -1)
                {
                    var linkInfo = effText.textInfo.linkInfo[link];
                    var reminderText = CardRepository.Reminders.KeywordToReminder[linkInfo.GetLinkID()];
                    //Debug.Log($"Hovering over {linkInfo.GetLinkID()} with reminder {reminderText}");
                    reminders.Add((linkInfo.GetLinkID(), reminderText));
                }
                ReminderTextsUIController.Show(reminders);
            }
        }


        protected override void DisplayCardRulesText()
        {
            nameText.text = shownCard.CardName;
            subtypesText.text = shownCard.QualifiedSubtypeText;
            effText.text = EffTextToDisplay;
        }

        protected override void DisplayCardNumericStats()
        {
            nText.text = $"N\n{shownCard.N}";
            eText.text = $"E\n{shownCard.E}";
            wText.text = $"W\n{shownCard.W}";

            switch (shownCard.CardType)
            {
                case 'C':
                    costText.text = $"S\n{shownCard.S}";

                    nText.gameObject.SetActive(true);
                    eText.gameObject.SetActive(true);
                    wText.gameObject.SetActive(true);
                    break;
                case 'S':
                    costText.text = $"C\n{shownCard.C}";

                    nText.gameObject.SetActive(false);
                    eText.gameObject.SetActive(false);
                    wText.gameObject.SetActive(false);
                    break;
                case 'A':
                    costText.text = $"A\n{shownCard.A}";

                    nText.gameObject.SetActive(false);
                    eText.gameObject.SetActive(false);
                    wText.gameObject.SetActive(false);
                    break;
                default: throw new System.ArgumentException("Failed to account for new card type in displaying card's numeric stats");
            }
        }

        protected override void DisplayCardImage()
        {
            string cardFileName = shownCard.FileName;
            var cardImageSprite = Resources.Load<Sprite>($"Simple Sprites/{cardFileName}");
            cardImageImage.sprite = cardImageSprite;
        }
    }
}