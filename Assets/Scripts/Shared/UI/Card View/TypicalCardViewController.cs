using KompasClient.UI;
using KompasCore.GameCore;
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

        [Header("Stat fonts")]
        [Tooltip("Default stat font")]
        public Material neutralStatFontMaterial;
        [Tooltip("Only needs to be specified if the outline should change when buffed")]
        public Material buffStatFontMaterial;
        [Tooltip("Only needs to be specified if the outline should change when debuffed")]
        public Material debuffStatFontMaterial;

        public abstract IReminderTextParentController ReminderTextsUIController { get; }

        protected virtual Camera Camera => Camera.main;
        protected virtual Vector3 ReminderTextMousePosition => Input.mousePosition;

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
                //check keywords
                int link = TMP_TextUtilities.FindIntersectingLink(effText, ReminderTextMousePosition, Camera);
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
                    break;
                case 'S':
                    costText.text = DisplayC(shownCard.C);

                    nText.gameObject.SetActive(false);
                    eText.gameObject.SetActive(false);
                    wText.gameObject.SetActive(false);
                    break;
                case 'A':
                    costText.text = DisplayA(shownCard.A);

                    nText.gameObject.SetActive(false);
                    eText.gameObject.SetActive(false);
                    wText.gameObject.SetActive(false);
                    break;
                default: throw new System.ArgumentException("Failed to account for new card type in displaying card's numeric stats");
            }

            handleStatColors(nText, eText, costText, wText);
        }

        protected void handleStatColors(TMP_Text n, TMP_Text e, TMP_Text cost, TMP_Text w)
        {
            ColorFromNumbers(n, (shownCard.N, shownCard.BaseN));
            ColorFromNumbers(e, (shownCard.E, shownCard.BaseE));
            ColorFromNumbers(w, (shownCard.W, shownCard.BaseW));

            ColorFromNumbers(cost, shownCard.CardType switch
            {
                'C' => (shownCard.S, shownCard.BaseS),
                'S' => (shownCard.C, shownCard.BaseC),
                'A' => (shownCard.A, shownCard.BaseA),
                _ => throw new System.ArgumentException("Failed to account for new card type in displaying card's numeric stats"),
            });
        }

        protected virtual void ColorFromNumbers(TMP_Text text, (int currStatValue, int baseStatValue) statValues)
        {
            text.fontMaterial = FontMaterialFromNumbers(statValues.currStatValue, statValues.baseStatValue);
        }

        private Material FontMaterialFromNumbers(int currStatValue, int baseStatValue)
        {
            if (currStatValue > baseStatValue) return buffStatFontMaterial;
            if (currStatValue < baseStatValue) return debuffStatFontMaterial;
            else return neutralStatFontMaterial;
        }

        protected virtual string DisplayN(int n) => $"N\n{n}";
        protected virtual string DisplayE(int e) => $"E\n{e}";
        protected virtual string DisplayS(int s) => $"S\n{s}";
        protected virtual string DisplayW(int w) => $"W\n{w}";
        protected virtual string DisplayC(int c) => $"C\n{c}";
        protected virtual string DisplayA(int a) => $"A\n{a}";

        protected sealed override void DisplayCardImage()
        {
            string cardFileName = shownCard.FileName;
            var cardImageSprite = CardRepository.LoadSprite(cardFileName);
            var cardImageTexture = CardRepository.LoadTexture(cardFileName);
            DisplayCardImage(cardImageSprite, cardImageTexture);
        }

        protected virtual void DisplayCardImage(Sprite cardImageSprite, Texture texture)
        {
            if (cardImageImage != null) cardImageImage.sprite = cardImageSprite;
        }
    }
}