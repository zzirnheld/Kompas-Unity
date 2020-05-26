using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TriggerRestriction
{
    public ServerSubeffect Subeffect;

    public enum TriggerRestrictions
    {
        ThisCardTriggered = 0,
        ThisCardInPlay = 1,
        AugmentedCardTriggered = 10,

        ThisCardFitsRestriction = 100,
        TriggererFitsRestriction = 101,

        ControllerTriggered = 200,
        EnemyTriggered = 201,

        //note: turns are the exception to the rule in that they are the only triggers where the triggering event
        //(in their case, the turn passing)
        //happens before the trigger is called, 
        //instead of the trigger being called at the moment before it happens.
        //in short, note that the turn will pass, then the trigger for turn start is called.
        //this means that checking for friendly/enemy turn will check whose turn the current (just-changed-to) turn is.
        FriendlyTurn = 300,
        EnemyTurn = 301,

        FromField = 400,
        FromDeck = 401
    }

    public TriggerRestrictions[] triggerRestrictions = new TriggerRestrictions[0];

    public CardRestriction cardRestriction = new CardRestriction();

    public Card thisCard { get; private set; }

    public ServerTrigger thisTrigger { get; private set; }

    public void Initialize(ServerSubeffect subeff, Card thisCard, ServerTrigger thisTrigger)
    {
        Subeffect = subeff;
        cardRestriction.Subeffect = subeff;
        this.thisCard = thisCard;
        this.thisTrigger = thisTrigger;
    }

    public bool Evaluate(Card cardTriggerer, IStackable stackTrigger, Player triggerer)
    {
        foreach(TriggerRestrictions r in triggerRestrictions)
        {
            switch (r)
            {
                case TriggerRestrictions.ThisCardTriggered:
                    if (cardTriggerer != thisCard) return false;
                    break;
                case TriggerRestrictions.ThisCardInPlay:
                    if (thisCard.Location != CardLocation.Field) return false;
                    break;
                case TriggerRestrictions.AugmentedCardTriggered:
                    if (!(thisCard is AugmentCard aug)) return false;
                    if (cardTriggerer != aug.AugmentedCard) return false;
                    break;
                case TriggerRestrictions.ThisCardFitsRestriction:
                    if (!cardRestriction.Evaluate(thisCard)) return false;
                    break;
                case TriggerRestrictions.TriggererFitsRestriction:
                    if (!cardRestriction.Evaluate(cardTriggerer)) return false;
                    break;
                //TODO make these into just something to do with triggered card fitting restriction
                case TriggerRestrictions.ControllerTriggered:
                    if (triggerer != thisCard.Controller) return false;
                    break;
                case TriggerRestrictions.EnemyTriggered:
                    if (triggerer == thisCard.Controller) return false;
                    break;
                case TriggerRestrictions.FriendlyTurn:
                    if (Subeffect.ServerGame.TurnPlayer != thisCard.Controller) return false;
                    break;
                case TriggerRestrictions.EnemyTurn:
                    if (Subeffect.ServerGame.TurnPlayer == thisCard.Controller) return false;
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
