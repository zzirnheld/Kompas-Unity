using UnityEngine;

namespace KompasCore.UI
{
    public class GameCardlikeViewController : TypicalCardViewController
    {
        [Header("Card highlighting")]
        public GameObject currentTargetObject;
        public GameObject validTargetObject;

        public override CardViewReminderTextParentController ReminderTextsUIController 
            => ShownCard?.Game.UIController.ReminderTextParentUIController;

        protected virtual void DisplaySpecialEffects()
        {
            currentTargetObject.SetActive(ShownCard.Game.IsCurrentTarget(ShownCard));
            validTargetObject.SetActive(ShownCard.Game.IsValidTarget(ShownCard));
        }
    }
}