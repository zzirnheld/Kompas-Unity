using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ServerTrigger : Trigger
{
    public ServerEffect effToTrigger;

    public (Card card, IStackable stack, Player player, int? x, (int, int)? space) LastTriggerInfo { get; private set; }

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
                TriggerDummySubeffect dummy = new TriggerDummySubeffect(parent);
                toReturn.triggerRestriction.Initialize(dummy, parent.Source, toReturn);
            }
        }

        return toReturn;
    }

    /// <summary>
    /// Pushes this trigger's effect onto the stack with the value of X if applicable.
    /// </summary>
    /// <param name="x"></param>
    protected void TriggerEffect(Card triggerer, IStackable stackable, Player player, int? x, (int, int)? space)
    {
        Debug.Log($"Triggering effect of {effToTrigger.Source.CardName} for value of x={x}");
        if (x.HasValue) effToTrigger.X = x.Value;
        if (triggerer != null) effToTrigger.Targets.Add(triggerer);
        //TODO should you notify right now about effect x? as of right now, no, because the important thing is the x value currently set in client network controller
        //and either another effect could be currently resolving with a different value of x
        //or the value of x could get changed between when this triggers and when the effect resolves
        LastTriggerInfo = (triggerer, stackable, player, x, space);
        effToTrigger.PushToStack(effToTrigger.serverGame.ServerPlayers[effToTrigger.Source.ControllerIndex]);
    }

    public void OverrideTrigger(int? x, ServerPlayer controller)
    {
        if (x.HasValue) effToTrigger.X = x.Value;
        effToTrigger.PushToStack(controller);
    }

    /// <summary>
    /// Checks all relevant trigger restrictions
    /// </summary>
    /// <param name="cardTriggerer">The card that triggered this, if any.</param>
    /// <param name="stackTrigger">The effect or attack that triggered this, if any.</param>
    /// <param name="x">If the action that triggered this has a value of x, it goes here. Otherwise, null.</param>
    /// <returns>Whether all restrictions of the trigger are fulfilled.</returns>
    protected bool CheckTriggerRestrictions(Card cardTriggerer, IStackable stackTrigger, Player triggerer, int? x, (int x, int y)? space)
    {
        if(triggerRestriction == null)
        {
            Debug.LogWarning($"Warning: null trigger restriction for effect of {effToTrigger.Source.CardName}");
        }

        return triggerRestriction.Evaluate(cardTriggerer: cardTriggerer, stackTrigger: stackTrigger, triggerer: triggerer, effX: x, space: space);
    }

    /// <summary>
    /// If the trigger for this effect applies to this trigger source, triggers this trigger's effect.
    /// </summary>
    /// <param name="cardTriggerer">The card that triggered this, if any.</param>
    /// <param name="stackTrigger">The effect or attack that triggered this, if any.</param>
    /// <param name="x">If the action that triggered this has a value of x, it goes here. Otherwise, null.</param>
    public virtual void TriggerIfValid(Card cardTriggerer, IServerStackable stackTrigger, ServerPlayer triggerer, int? x, (int, int)? space)
    {
        /*Debug.Log($"Is trigger valid for effect of {effToTrigger.thisCard.CardName} with id {effToTrigger.thisCard.ID}? " +
            $"{CheckTriggerRestrictions(triggerer, stackTrigger, x)}");*/
        if (CheckTriggerRestrictions(cardTriggerer, stackTrigger, triggerer, x, space))
        {
            Debug.Log($"Trigger is valid for effect of {effToTrigger.Source.CardName} with id {effToTrigger.Source.ID}");
            if (Optional) effToTrigger.serverGame.EffectsController
                     .AskForTrigger(this, x, cardTriggerer, stackTrigger, triggerer, effToTrigger.serverGame.ServerPlayers[effToTrigger.Source.ControllerIndex]);
            else TriggerEffect(cardTriggerer, stackTrigger, triggerer, x, space);
        }
    }
}
