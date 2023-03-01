using KompasCore.Cards;
using TMPro;
using UnityEngine;

namespace KompasCore.UI
{
    public class GameCardlikeViewController : TypicalCardViewController
    {
        private static readonly Color32 NormalColor = new Color32(0, 0, 0, 255);
        private static readonly Color32 BuffColor = new Color32(0, 255, 0, 255);
        private static readonly Color32 DebuffColor = new Color32(255, 0, 0, 255);

        [Header("Card highlighting")]
        public GameObject currentTargetObject;
        public GameObject validTargetObject;
        public GameObject focusedCardObject;

        protected GameCard ShownGameCard => shownCard as GameCard;

        public override IReminderTextParentController ReminderTextsUIController 
            => ShownGameCard?.Game.UIController.ReminderTextParentUIController;

        protected override void Display()
        {
            base.Display();

            DisplaySpecialEffects();
        }

        protected virtual void DisplaySpecialEffects()
        {
            currentTargetObject.SetActive(ShownGameCard.Game.IsCurrentTarget(ShownGameCard));
            validTargetObject.SetActive(ShownGameCard.Game.IsValidTarget(ShownGameCard));

            focusedCardObject.SetActive(ShownGameCard.Game.FocusedCard == FocusedCard);
        }

        protected override void ColorFromNumbers(TMP_Text text, (int currStatValue, int baseStatValue) statValues)
        {
            base.ColorFromNumbers(text, statValues);
            text.color = FontColorFromNumbers(statValues.currStatValue, statValues.baseStatValue);
        }

        private Color32 FontColorFromNumbers(int currStatValue, int baseStatValue)
        {
            if (currStatValue < baseStatValue) return DebuffColor;
            if (currStatValue > baseStatValue) return BuffColor;
            else return NormalColor;
        }
    }
}