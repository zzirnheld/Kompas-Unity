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
        FromField = 400,
        FromDeck = 401
    }

    public TriggerRestrictions[] triggerRestrictions = new TriggerRestrictions[0];

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
                    //TODO make these into just something to do with triggered card fitting restriction
                case TriggerRestrictions.FromField:
                    if (triggerer.Location != CardLocation.Field) return false;
                    break;
                case TriggerRestrictions.FromDeck:
                    if (triggerer.Location != CardLocation.Deck) return false;
                    break;
                default:
                    Debug.LogError($"Unrecognized trigger restriction {r}");
                    break;
            }
        }

        return true;
    }
}
