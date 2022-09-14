using KompasCore.Cards;
using UnityEngine;

namespace KompasCore.UI
{
    public class GameCardlikeViewController : TypicalCardViewController
    {
        [Header("Card highlighting")]
        public GameObject currentTargetObject;
        public GameObject validTargetObject;

        protected GameCard ShownGameCard => shownCard as GameCard;

        public override IReminderTextParentController ReminderTextsUIController 
            => ShownGameCard?.Game.UIController.ReminderTextParentUIController;

        protected virtual void DisplaySpecialEffects()
        {
            currentTargetObject.SetActive(ShownGameCard.Game.IsCurrentTarget(ShownGameCard));
            validTargetObject.SetActive(ShownGameCard.Game.IsValidTarget(ShownGameCard));
        }
    }
}