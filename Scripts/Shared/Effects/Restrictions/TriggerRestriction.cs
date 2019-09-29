using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TriggerRestriction : Restriction
{
    public enum TriggerRestrictions
    {
        ThisCardTriggered = 0,
        EffectParentRestriction = 100,
        WasOnField = 400,
    }

    public TriggerRestrictions[] triggerRestrictions;

    public CardRestriction effParentRestriction;

    [System.NonSerialized]
    public Card thisCard;

    [System.NonSerialized]
    public Trigger thisTrigger;

    public bool Evaluate(Card triggerer, Effect effTrigger, Attack atkTrigger)
    {
        foreach(TriggerRestrictions r in triggerRestrictions)
        {
            switch (r)
            {
                case TriggerRestrictions.ThisCardTriggered:
                    if (triggerer != thisTrigger.effToTrigger.thisCard) return false;
                    break;
                case TriggerRestrictions.EffectParentRestriction:
                    if (effParentRestriction == null) return false;
                    if (!effParentRestriction.Evaluate(thisCard)) return false;
                    break;
                case TriggerRestrictions.WasOnField:
                    if (triggerer.Location != Card.CardLocation.Field) return false;
                    break;
            }
        }

        return true;
    }
}
