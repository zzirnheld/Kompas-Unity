using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TriggerRestriction
{
    public Subeffect Subeffect;

    public enum TriggerRestrictions
    {
        ThisCardTriggered = 0,
        ThisCardFitsRestriction = 100,
        TriggererFitsRestriction = 101,
        OnField = 400,
    }

    public TriggerRestrictions[] triggerRestrictions;

    public CardRestriction cardRestriction;

    [System.NonSerialized]
    public Card thisCard;

    [System.NonSerialized]
    public Trigger thisTrigger;

    public bool Evaluate(Card triggerer, IStackable stackTrigger)
    {
        foreach(TriggerRestrictions r in triggerRestrictions)
        {
            switch (r)
            {
                case TriggerRestrictions.ThisCardTriggered:
                    if (triggerer != thisTrigger.effToTrigger.thisCard) return false;
                    break;
                case TriggerRestrictions.ThisCardFitsRestriction:
                    if (!cardRestriction.Evaluate(thisCard)) return false;
                    break;
                case TriggerRestrictions.OnField:
                    if (triggerer.Location != CardLocation.Field) return false;
                    break;
            }
        }

        return true;
    }
}
