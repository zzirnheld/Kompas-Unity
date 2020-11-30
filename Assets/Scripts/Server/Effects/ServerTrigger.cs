using KompasCore.Cards;
using KompasCore.Effects;
using System.Linq;
using UnityEngine;

namespace KompasServer.Effects
{
    [System.Serializable]
    public class ServerTrigger : Trigger
    {
        public ServerEffect serverEffect;

        public override GameCard Source => serverEffect.Source;
        public override Effect Effect => serverEffect;

        private bool responded = false;
        /// <summary>
        /// Represents whether this trigger, if optional, has been responded to (to accept or decline)
        /// </summary>
        public bool Responded
        {
            get => !Optional || responded;
            set => responded = value;
        }

        private bool confirmed = false;
        /// <summary>
        /// Represents whether this trigger, if optional, was chosen to be used or not.
        /// </summary>
        public bool Confirmed
        {
            get => !Optional || confirmed;
            set => confirmed = value;
        }

        /// <summary>
        /// Represents the order this trigger has been given, amongst other simultaneously triggered triggers.
        /// </summary>
        private int order = -1;
        public int Order
        {
            get => order;
            set
            {
                order = value;
                Responded = true;
            }
        }
        public bool Ordered => order != -1;

        public ServerTrigger(TriggerData triggerData, ServerEffect parent) : base(triggerData, parent.Game)
        {
            serverEffect = parent;
            if(!TriggerConditions.Contains(triggerData.triggerCondition))
                throw new System.ArgumentNullException("triggerRestriction", $"null trigger restriction for effect of {parent.Source.CardName}");
        }

        /// <summary>
        /// Checks all relevant trigger restrictions, and whether the card is negated
        /// </summary>
        /// <param name="cardTriggerer">The card that triggered this, if any.</param>
        /// <param name="stackTrigger">The effect or attack that triggered this, if any.</param>
        /// <param name="x">If the action that triggered this has a value of x, it goes here. Otherwise, null.</param>
        /// <returns>Whether all restrictions of the trigger are fulfilled.</returns>
        public bool ValidForContext(ActivationContext context) => !Source.Negated && TriggerRestriction.Evaluate(context);

        /// <summary>
        /// Rechecks any trigger restrictions that might have changed between the trigger triggering and being ordered.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool StillValidForContext(ActivationContext context) => TriggerRestriction.Reevaluate(context);

        /// <summary>
        /// Resets Confirmed and Responded, for the next time this effect might be triggered
        /// </summary>
        public void ResetConfirmation()
        {
            Responded = false;
            Confirmed = false;
            order = -1;
        }

        /// <summary>
        /// Asks this effect's controller if they want to trigger this trigger.
        /// Should only be called for optional triggers - anything else doesn't make sense
        /// </summary>
        public void Ask()
        {
            if (!Optional) 
                throw new System.InvalidOperationException("Can't ask the player to okay a trigger that isn't optional");

            serverEffect.ServerController.ServerNotifier.AskForTrigger(this);
        }

        /// <summary>
        /// Updates whether this trigger will be triggered, based on the player's answer
        /// </summary>
        /// <param name="answerer"></param>
        public void Answered(bool answer, Player answerer)
        {
            if (!Optional) 
                throw new System.InvalidOperationException("Can't answer a trigger that isn't optional");
            if (answerer != Effect.Controller) 
                throw new System.InvalidOperationException("Player other than the owner tried to answer a trigger");

            Confirmed = answer;
            Responded = true;
        }
    }
}