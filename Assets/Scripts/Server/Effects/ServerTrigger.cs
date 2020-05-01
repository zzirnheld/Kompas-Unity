using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public static ServerTrigger FromJson(TriggerCondition condition, string json, ServerEffect parent)
    {
        ServerTrigger toReturn = JsonUtility.FromJson<ServerTrigger>(json);

        if(toReturn != null)
        {
            //set all values shared by all triggers
            toReturn.triggerCondition = condition;
            toReturn.effToTrigger = parent;
            //if the trigger has any restriction, set its values
            if(toReturn.triggerRestriction != null)
            {
                toReturn.triggerRestriction.thisTrigger = toReturn;
                toReturn.triggerRestriction.thisCard = parent.thisCard;
                if(toReturn.triggerRestriction.cardRestriction != null)
                {
                    //give a dummy subeffect to the trigger restriction's card restriction,
                    //because the restriction isn't bound to a subeffect, only an effect
                    TriggerDummySubeffect dummy = new TriggerDummySubeffect(parent);
                    toReturn.triggerRestriction.cardRestriction.Subeffect = dummy;
                }
            }
        }

        return toReturn;
    }

    /// <summary>
    /// Pushes this trigger's effect onto the stack with the value of X if applicable.
    /// </summary>
    /// <param name="x"></param>
    protected void TriggerEffect(int? x)
    {
        Debug.Log($"Triggering effect of {effToTrigger.thisCard.CardName} for value of x={x}");
        if (x.HasValue) effToTrigger.X = x.Value;
        //TODO should you notify right now about effect x? as of right now, no, because the important thing is the x value currently set in client network controller
        //and either another effect could be currently resolving with a different value of x
        //or the value of x could get changed between when this triggers and when the effect resolves
        effToTrigger.PushToStack(effToTrigger.serverGame.ServerPlayers[effToTrigger.thisCard.ControllerIndex]);
    }

    /// <summary>
    /// Checks all relevant trigger restrictions
    /// </summary>
    /// <param name="cardTriggerer">The card that triggered this, if any.</param>
    /// <param name="stackTrigger">The effect or attack that triggered this, if any.</param>
    /// <param name="x">If the action that triggered this has a value of x, it goes here. Otherwise, null.</param>
    /// <returns>Whether all restrictions of the trigger are fulfilled.</returns>
    protected bool CheckTriggerRestrictions(Card cardTriggerer, IStackable stackTrigger, int? x, Player triggerer)
    {
        if (effToTrigger.MaxTimesCanUsePerTurn.HasValue &&
            effToTrigger.TimesUsedThisTurn >= effToTrigger.MaxTimesCanUsePerTurn)
            return false;

        if(triggerRestriction == null)
        {
            Debug.LogWarning($"Warning: null trigger restriction for effect of {effToTrigger.thisCard.CardName}");
        }

        return triggerRestriction.Evaluate(cardTriggerer, stackTrigger, triggerer);
    }

    /// <summary>
    /// If the trigger for this effect applies to this trigger source, triggers this trigger's effect.
    /// </summary>
    /// <param name="cardTriggerer">The card that triggered this, if any.</param>
    /// <param name="stackTrigger">The effect or attack that triggered this, if any.</param>
    /// <param name="x">If the action that triggered this has a value of x, it goes here. Otherwise, null.</param>
    public virtual void TriggerIfValid(Card cardTriggerer, IStackable stackTrigger, int? x, ServerPlayer triggerer, bool optionalConfirmed = false)
    {
        /*Debug.Log($"Is trigger valid for effect of {effToTrigger.thisCard.CardName} with id {effToTrigger.thisCard.ID}? " +
            $"{CheckTriggerRestrictions(triggerer, stackTrigger, x)}");*/
        if (CheckTriggerRestrictions(cardTriggerer, stackTrigger, x, triggerer))
        {
            Debug.Log($"Trigger is valid for effect of {effToTrigger.thisCard.CardName} with id {effToTrigger.thisCard.ID}");
            //if the trigger is optional and this method isn't being called because the player confirmed the trigger,
            //then ask for a trigger
            if (Optional && !optionalConfirmed)
            {
                effToTrigger.serverGame.AskForTrigger(this, x, cardTriggerer, stackTrigger, triggerer);
            }
            else TriggerEffect(x);
        }
    }
}
