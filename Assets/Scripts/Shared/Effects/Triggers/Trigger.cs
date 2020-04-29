using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Trigger
{
    public TriggerCondition triggerCondition;
    public Effect effToTrigger;
    public TriggerRestriction triggerRestriction;

    public bool Optional = false;
    public string Blurb = "";

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
            case TriggerCondition.DrawX:
            case TriggerCondition.EachDraw:
                toReturn = JsonUtility.FromJson<Trigger>(json);
                break;
            default:
                Debug.LogError($"Unrecognized trigger condition for {parent.thisCard.CardName}");
                toReturn = JsonUtility.FromJson<Trigger>(json);
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
                if(toReturn.triggerRestriction.cardRestriction != null)
                {
                    //give a dummy subeffect to the trigger restriction's card restriction,
                    //because the restriction isn't bound to a subeffect, only an effect
                    DummySubeffect dummy = new DummySubeffect
                    {
                        Effect = parent
                    };
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
        if (x.HasValue) effToTrigger.X = x.Value;
        effToTrigger.PushToStack(effToTrigger.thisCard.ControllerIndex);
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
