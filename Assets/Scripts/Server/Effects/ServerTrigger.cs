using KompasCore.Cards;
using KompasCore.Effects;
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
            get => !optional || responded;
            set => responded = value;
        }

        private bool confirmed = false;
        /// <summary>
        /// Represents whether this trigger, if optional, was chosen to be used or not.
        /// </summary>
        public bool Confirmed
        {
            get => !optional || confirmed;
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

        /// <summary>
        /// Creates a trigger from a json
        /// </summary>
        /// <param name="condition">The condition on which to trigger the effect.</param>
        /// <param name="json">The json to create the object from.</param>
        /// <param name="parent">The effect this trigger will trigger.</param>
        /// <returns>The trigger with these characteristics.</returns>
        public static ServerTrigger FromJson(string condition, string json, ServerEffect parent)
        {
            ServerTrigger toReturn = JsonUtility.FromJson<ServerTrigger>(json);

            //set all values shared by all triggers
            toReturn.triggerCondition = condition;
            toReturn.serverEffect = parent;
            //it also better have a trigger restriction, even if it's a generic one
            if (toReturn.triggerRestriction == null) 
                    throw new System.ArgumentNullException($"null trigger restriction for effect of {toReturn.serverEffect.Source.CardName}");
            var dummy = new TriggerDummySubeffect(parent);
            toReturn.triggerRestriction.Initialize(dummy, parent.Source, toReturn);

            return toReturn;
        }

        /// <summary>
        /// Checks all relevant trigger restrictions
        /// </summary>
        /// <param name="cardTriggerer">The card that triggered this, if any.</param>
        /// <param name="stackTrigger">The effect or attack that triggered this, if any.</param>
        /// <param name="x">If the action that triggered this has a value of x, it goes here. Otherwise, null.</param>
        /// <returns>Whether all restrictions of the trigger are fulfilled.</returns>
        public bool ValidForContext(ActivationContext context) => triggerRestriction.Evaluate(context);

        /// <summary>
        /// Rechecks any trigger restrictions that might have changed between the trigger triggering and being ordered.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool StillValidForContext(ActivationContext context) => triggerRestriction.Reevaluate(context);

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
            if (!optional) 
                throw new System.InvalidOperationException("Can't ask the player to okay a trigger that isn't optional");

            serverEffect.ServerController.ServerNotifier.AskForTrigger(this);
        }

        /// <summary>
        /// Updates whether this trigger will be triggered, based on the player's answer
        /// </summary>
        /// <param name="answerer"></param>
        public void Answered(bool answer, Player answerer)
        {
            if (!optional) 
                throw new System.InvalidOperationException("Can't answer a trigger that isn't optional");
            if (answerer != Effect.Controller) 
                throw new System.InvalidOperationException("Player other than the owner tried to answer a trigger");

            Confirmed = answer;
            Responded = true;
        }
    }
}