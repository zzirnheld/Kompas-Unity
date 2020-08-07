using KompasCore.Effects;
using KompasServer.GameCore;
using UnityEngine;

namespace KompasServer.Effects
{
    [System.Serializable]
    public class ServerTrigger : Trigger
    {
        public ServerEffect effToTrigger;

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
        /// Creates a trigger from a json
        /// </summary>
        /// <param name="condition">The condition on which to trigger the effect.</param>
        /// <param name="json">The json to create the object from.</param>
        /// <param name="parent">The effect this trigger will trigger.</param>
        /// <returns>The trigger with these characteristics.</returns>
        public static ServerTrigger FromJson(string condition, string json, ServerEffect parent)
        {
            ServerTrigger toReturn = JsonUtility.FromJson<ServerTrigger>(json);

            if (toReturn != null)
            {
                //set all values shared by all triggers
                toReturn.triggerCondition = condition;
                toReturn.effToTrigger = parent;
                //if the trigger has any restriction, set its values
                if (toReturn.triggerRestriction != null)
                {
                    var dummy = new TriggerDummySubeffect(parent);
                    toReturn.triggerRestriction.Initialize(dummy, parent.Source, toReturn);
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Checks all relevant trigger restrictions
        /// </summary>
        /// <param name="cardTriggerer">The card that triggered this, if any.</param>
        /// <param name="stackTrigger">The effect or attack that triggered this, if any.</param>
        /// <param name="x">If the action that triggered this has a value of x, it goes here. Otherwise, null.</param>
        /// <returns>Whether all restrictions of the trigger are fulfilled.</returns>
        public bool ValidForContext(ActivationContext context)
        {
            if (triggerRestriction == null) throw new System.ArgumentNullException($"null trigger restriction for effect of {effToTrigger.Source.CardName}");

            return triggerRestriction.Evaluate(context);
        }

        /// <summary>
        /// Resets Confirmed and Responded, for the next time this effect might be triggered
        /// </summary>
        public void ResetConfirmation()
        {
            Responded = false;
            Confirmed = false;
        }
    }
}