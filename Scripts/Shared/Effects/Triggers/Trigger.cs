using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Trigger
{
    public TriggerCondition TriggerCondition;
    [System.NonSerialized]
    public Effect effToTrigger;

    public static Trigger FromJson(TriggerCondition c, string json, Effect parent)
    {
        Trigger toReturn = null;

        switch (c)
        {
            case TriggerCondition.TurnStart:
                toReturn = JsonUtility.FromJson<Trigger>(json);
                break;
        }


        if(toReturn != null)
        {
            toReturn.TriggerCondition = c;
            toReturn.effToTrigger = parent;
        }

        return toReturn;
    }

    public virtual void TriggerIfValid(Effect trigger, int x)
    {
        if (trigger.TimesUsedThisTurn >= trigger.MaxTimesCanUsePerTurn) return;
        effToTrigger.X = x;
        effToTrigger.serverGame.PushToStack(effToTrigger, effToTrigger.thisCard.ControllerIndex);
    }
}
