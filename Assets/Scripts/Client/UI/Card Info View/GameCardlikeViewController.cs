using KompasCore.Cards;
using UnityEngine;

namespace KompasCore.UI
{
    public class GameCardlikeViewController : TypicalCardViewController
    {
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
    }
}