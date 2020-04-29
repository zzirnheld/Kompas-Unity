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
        ThisCardInPlay = 1,
        ThisCardFitsRestriction = 100,
        TriggererFitsRestriction = 101,
        ControllerTriggered = 200,
        EnemyTriggered = 201,
        FromField = 400,
        FromDeck = 401
    }

    public TriggerRestrictions[] triggerRestrictions = new TriggerRestrictions[0];

    public CardRestriction cardRestriction = new CardRestriction();

    [System.NonSerialized]
    public Card thisCard;

    [System.NonSerialized]
    public Trigger thisTrigger;

    public void Initialize(Subeffect subeff)
    {
        Subeffect = subeff;
        cardRestriction.Subeffect = subeff;
    }

    public bool Evaluate(Card cardTriggerer, IStackable stackTrigger, Player triggerer)
    {
        foreach(TriggerRestrictions r in triggerRestrictions)
        {
            switch (r)
            {
                case TriggerRestrictions.ThisCardTriggered:
                    if (cardTriggerer != thisTrigger.effToTrigger.thisCard) return false;
                    break;
                case TriggerRestrictions.ThisCardInPlay:
                    if (thisCard.Location != CardLocation.Field) return false;
                    break;
                case TriggerRestrictions.ThisCardFitsRestriction:
                    if (!cardRestriction.Evaluate(thisCard)) return false;
                    break;
                //TODO make these into just something to do with triggered card fitting restriction
                case TriggerRestrictions.ControllerTriggered:
                    if (triggerer != thisCard.Controller) return false;
                    break;
                case TriggerRestrictions.EnemyTriggered:
                    if (triggerer == thisCard.Controller) return false;
                    break;
                case TriggerRestrictions.FromField:
                    if (cardTriggerer.Location != CardLocation.Field) return false;
                    break;
                case TriggerRestrictions.FromDeck:
                    if (cardTriggerer.Location != CardLocation.Deck) return false;
                    break;
                default:
                    Debug.LogError($"Unrecognized trigger restriction {r}");
                    break;
            }
        }

        return true;
    }
}
