using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Trigger
{
    public TriggerCondition triggerCondition;
    public Effect effToTrigger;
    public TriggerRestriction triggerRestriction;

    /// <summary>
    /// Creates a trigger from a json
    /// </summary>
    /// <param name="condition">The condition on which to trigger the effect.</param>
    /// <param name="json">The json to create the object from.</param>
    /// <param name="parent">The effect this trigger will trigger.</param>
    /// <returns>The trigger with these characteristics.</returns>
    public static Trigger FromJson(TriggerCondition condition, string json, Effect parent)
    {
        Trigger toReturn = null;

        Debug.Log($"Deserializing trigger json \n{json}");
        switch (condition)
        {
            //So far, everything is just a normal trigger, but that could change for other types of trigger restrictions
            case TriggerCondition.TurnStart:
            case TriggerCondition.Play:
            case TriggerCondition.Discard:
                toReturn = JsonUtility.FromJson<Trigger>(json);
                break;
            default:
                Debug.Log($"Unrecognized trigger condition for {parent.thisCard.CardName}");
                break;
        }

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
                if(toReturn.triggerRestriction.effParentRestriction != null)
                {
                    //give a dummy subeffect to the trigger restriction since it's a trigger that's not bound to an eff
                    DummySubeffect dummy = new DummySubeffect
                    {
                        parent = parent
                    };
                    toReturn.triggerRestriction.effParentRestriction.subeffect = dummy;
                    
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
        if (x.HasValue) effToTrigger.X = x.Value;
        effToTrigger.PushToStack(effToTrigger.thisCard.ControllerIndex);
    }

    /// <summary>
    /// Checks all relevant trigger restrictions
    /// </summary>
    /// <param name="triggerer">The card that triggered this, if any.</param>
    /// <param name="stackTrigger">The effect or attack that triggered this, if any.</param>
    /// <param name="x">If the action that triggered this has a value of x, it goes here. Otherwise, null.</param>
    /// <returns>Whether all restrictions of the trigger are fulfilled.</returns>
    protected bool CheckTriggerRestrictions(Card triggerer, IStackable stackTrigger, int? x)
    {
        if (effToTrigger.MaxTimesCanUsePerTurn.HasValue &&
            effToTrigger.TimesUsedThisTurn >= effToTrigger.MaxTimesCanUsePerTurn)
            return false;

        if(triggerRestriction == null)
        {
            Debug.LogWarning($"Warning: null trigger restriction for effect of {effToTrigger.thisCard.CardName}");
        }

        return triggerRestriction.Evaluate(triggerer, stackTrigger);
    }

    /// <summary>
    /// If the trigger for this effect applies to this trigger source, triggers this trigger's effect.
    /// </summary>
    /// <param name="triggerer">The card that triggered this, if any.</param>
    /// <param name="stackTrigger">The effect or attack that triggered this, if any.</param>
    /// <param name="x">If the action that triggered this has a value of x, it goes here. Otherwise, null.</param>
    public virtual void TriggerIfValid(Card triggerer, IStackable stackTrigger, int? x)
    {
        Debug.Log($"Is trigger valid? {CheckTriggerRestrictions(triggerer, stackTrigger, x)}");
        if (CheckTriggerRestrictions(triggerer, stackTrigger, x))
            TriggerEffect(x);
    }
}
