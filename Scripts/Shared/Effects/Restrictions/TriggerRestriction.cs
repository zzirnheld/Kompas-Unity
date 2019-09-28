using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TriggerRestriction : Restriction
{
    public enum TriggerRestrictions
    {
        EffectParentRestriction = 100,
        WasOnField = 400
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
