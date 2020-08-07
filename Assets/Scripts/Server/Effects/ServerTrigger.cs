using KompasCore.Effects;
using KompasServer.GameCore;
using UnityEngine;

namespace KompasServer.Effects
{
    [System.Serializable]
    public class ServerTrigger : Trigger
    {
        public ServerEffect effToTrigger;

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
        /// Pushes this trigger's effect onto the stack with the value of X if applicable.
        /// </summary>
        /// <param name="x"></param>
        protected void TriggerEffect(ActivationContext context)
        {
            Debug.Log($"Triggering effect of {effToTrigger.Source.CardName} for context {context}");
            //TODO should you notify right now about effect x? as of right now, no, because the important thing is the x value currently set in client network controller
            //and either another effect could be currently resolving with a different value of x
            //or the value of x could get changed between when this triggers and when the effect resolves
            effToTrigger.PushToStack(effToTrigger.serverGame.ServerPlayers[effToTrigger.Source.ControllerIndex], context);
        }

        /// <summary>
        /// Trigger this effect, ignoring the trigger restriction
        /// </summary>
        /// <param name="context"></param>
        /// <param name="controller"></param>
        public void OverrideTrigger(ActivationContext context, ServerPlayer controller) => effToTrigger.PushToStack(controller, context);

        /// <summary>
        /// Checks all relevant trigger restrictions
        /// </summary>
        /// <param name="cardTriggerer">The card that triggered this, if any.</param>
        /// <param name="stackTrigger">The effect or attack that triggered this, if any.</param>
        /// <param name="x">If the action that triggered this has a value of x, it goes here. Otherwise, null.</param>
        /// <returns>Whether all restrictions of the trigger are fulfilled.</returns>
        protected bool CheckTriggerRestrictions(ActivationContext context)
        {
            if (triggerRestriction == null)
            {
                Debug.LogWarning($"Warning: null trigger restriction for effect of {effToTrigger.Source.CardName}");
            }

            return triggerRestriction.Evaluate(context);
        }

        /// <summary>
        /// If the trigger for this effect applies to this trigger source, triggers this trigger's effect.
        /// </summary>
        /// <param name="cardTriggerer">The card that triggered this, if any.</param>
        /// <param name="stackTrigger">The effect or attack that triggered this, if any.</param>
        /// <param name="x">If the action that triggered this has a value of x, it goes here. Otherwise, null.</param>
        public void TriggerIfValid(ActivationContext context)
        {
            /*Debug.Log($"Is trigger valid for effect of {effToTrigger.thisCard.CardName} with id {effToTrigger.thisCard.ID}? " +
                $"{CheckTriggerRestrictions(triggerer, stackTrigger, x)}");*/
            if (CheckTriggerRestrictions(context))
            {
                Debug.Log($"Trigger is valid for effect of {effToTrigger.Source.CardName} with id {effToTrigger.Source.ID}");
                if (optional) effToTrigger.serverGame.EffectsController
                         .AskForTrigger(this, context, effToTrigger.serverGame.ServerPlayers[effToTrigger.Source.ControllerIndex]);
                else TriggerEffect(context);
            }
        }
    }
}