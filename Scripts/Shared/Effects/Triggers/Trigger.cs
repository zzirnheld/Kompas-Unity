using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Trigger
{
    public TriggerCondition triggerCondition;
    public Effect effToTrigger;
    public TriggerRestriction triggerRestriction;

    public static Trigger FromJson(TriggerCondition c, string json, Effect parent)
    {
        Trigger toReturn = null;

        Debug.Log("Deserializing trigger \n" + json);
        switch (c)
        {
            case TriggerCondition.TurnStart:
            case TriggerCondition.Play:
            case TriggerCondition.Discard:
                toReturn = JsonUtility.FromJson<Trigger>(json);
                break;
            default:
                Debug.Log("unrecognized trigger conditoin for " + parent.thisCard.CardName);
                break;
        }

        if(toReturn != null)
        {
            toReturn.triggerCondition = c;
            toReturn.effToTrigger = parent;
            if(toReturn.triggerRestriction != null)
            {
                toReturn.triggerRestriction.thisTrigger = toReturn;
                toReturn.triggerRestriction.thisCard = parent.thisCard;
                if(toReturn.triggerRestriction.effParentRestriction != null)
                {
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

    protected void TriggerEffect(int? x)
    {
        if (x.HasValue) effToTrigger.X = x.Value;
        effToTrigger.PushToStack(false);
    }

    protected bool CheckTriggerRestrictions(Card triggerer, Effect effTrigger, Attack attackTrigger, int? x)
    {
        if (effToTrigger.MaxTimesCanUsePerTurn.HasValue &&
            effToTrigger.TimesUsedThisTurn >= effToTrigger.MaxTimesCanUsePerTurn)
            return false;
        return triggerRestriction.Evaluate(triggerer, effTrigger, attackTrigger);
    }

    public virtual void TriggerIfValid(Card triggerer, Effect effTrigger, Attack attackTrigger, int? x)
    {
        if (CheckTriggerRestrictions(triggerer, effTrigger, attackTrigger, x))
            TriggerEffect(x);
    }
}
