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

        [Header("Misc all typical cards")]
        public ReminderTextParentUIController reminderTextsUIController;

        protected override void DisplayNothing()
        {
            base.DisplayNothing();

            //TODO make these each linked to their own CardViewController, so they can leverage the TextMeshPro stuff I googled to make tooltip boxes
            reminderTextsUIController?.ShowNothing();
        }

        protected override void Display()
        {
            base.Display();

            reminderTextsUIController?.ShowReminderText(ShownCard);
        }

        protected override void DisplayCardRulesText()
        {
            nameText.text = ShownCard.CardName;
            subtypesText.text = ShownCard.QualifiedSubtypeText;
            effText.text = ShownCard.EffText;
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