using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Trigger
{
    public TriggerCondition TriggerCondition;
    [System.NonSerialized]
    public Effect effToTrigger;

    public virtual void TriggerIfValid(Effect trigger, int x)
    {
        if (trigger.TimesUsedThisTurn >= trigger.MaxTimesCanUsePerTurn) return;
        effToTrigger.X = x;
        effToTrigger.serverGame.PushToStack(effToTrigger, effToTrigger.thisCard.ControllerIndex);
    }
}
