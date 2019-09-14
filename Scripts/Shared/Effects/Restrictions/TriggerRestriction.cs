using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TriggerRestriction : Restriction
{
    public enum TriggerRestrictions
    {
        EffectParentRestriction = 100
    }

    public TriggerRestrictions[] triggerRestrictions;

    public CardRestriction effParentRestriction;

    [System.NonSerialized]
    public Card thisCard;

    [System.NonSerialized]
    public Trigger thisTrigger;

    public override bool Evaluate()
    {
        foreach(TriggerRestrictions r in triggerRestrictions)
        {
            switch (r)
            {
                case TriggerRestrictions.EffectParentRestriction:
                    if (effParentRestriction == null) return false;
                    if (!effParentRestriction.Evaluate(thisCard)) return false;
                    break;
            }
        }

        return true;
    }
}
