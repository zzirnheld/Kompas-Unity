using KompasClient.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KompasCore.UI
{
    public abstract class TypicalCardViewController : BaseCardViewController
    {
        public static readonly Color32 BUFF_OUTLINE_COLOR = new Color32(255, 0, 0, 255);
        public static readonly Color32 DEBUFF_OUTLINE_COLOR = new Color32(0, 255, 0, 255); //TODO make customizeable
        public static readonly Color32 BASE_OUTLINE_COLOR = new Color32(0, 0, 0, 255);

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
            if (ReminderTextsUIController != null && effText != null && effText.isActiveAndEnabled)
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

        private Color32 ColorFromNumbers(int currStatValue, int baseStatValue)
        {
            if (currStatValue > baseStatValue) return BUFF_OUTLINE_COLOR;
            if (currStatValue < baseStatValue) return DEBUFF_OUTLINE_COLOR;
            else return BASE_OUTLINE_COLOR;
        }

        protected override void DisplayCardNumericStats()
        {
            switch (shownCard.CardType)
            {
                case 'C':
                    nText.text = DisplayN(shownCard.N);
                    eText.text = DisplayE(shownCard.E);
                    wText.text = DisplayW(shownCard.W);
                    costText.text = DisplayS(shownCard.S);

                    nText.gameObject.SetActive(true);
                    eText.gameObject.SetActive(true);
                    wText.gameObject.SetActive(true);

                    nText.outlineColor = ColorFromNumbers(shownCard.N, shownCard.BaseN);
                    eText.outlineColor = ColorFromNumbers(shownCard.E, shownCard.BaseE);
                    costText.outlineColor = ColorFromNumbers(shownCard.S, shownCard.BaseS);
                    wText.outlineColor = ColorFromNumbers(shownCard.W, shownCard.BaseW);
                    break;
                case 'S':
                    costText.text = DisplayC(shownCard.C);

                    nText.gameObject.SetActive(false);
                    eText.gameObject.SetActive(false);
                    wText.gameObject.SetActive(false);

                    costText.outlineColor = ColorFromNumbers(shownCard.C, shownCard.BaseC);
                    break;
                case 'A':
                    costText.text = DisplayA(shownCard.A);

                    nText.gameObject.SetActive(false);
                    eText.gameObject.SetActive(false);
                    wText.gameObject.SetActive(false);

                    costText.outlineColor = ColorFromNumbers(shownCard.A, shownCard.BaseA);
                    break;
                default: throw new System.ArgumentException("Failed to account for new card type in displaying card's numeric stats");
            }
        }

        protected virtual string DisplayN(int n) => $"N\n{n}";
        protected virtual string DisplayE(int e) => $"E\n{e}";
        protected virtual string DisplayS(int s) => $"S\n{s}";
        protected virtual string DisplayW(int w) => $"W\n{w}";
        protected virtual string DisplayC(int c) => $"C\n{c}";
        protected virtual string DisplayA(int a) => $"A\n{a}";

        protected override void DisplayCardImage()
        {
            string cardFileName = shownCard.FileName;
            var cardImageSprite = Resources.Load<Sprite>($"Simple Sprites/{cardFileName}");
            cardImageImage.sprite = cardImageSprite;
        }
    }
}