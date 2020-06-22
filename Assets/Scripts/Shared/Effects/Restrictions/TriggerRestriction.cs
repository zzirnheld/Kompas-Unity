using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TriggerRestriction
{
    public ServerSubeffect Subeffect { get; private set; }

    public enum TriggerRestrictions
    {
        ThisCardTriggered = 0,
        ThisCardInPlay = 1,
        AugmentedCardTriggered = 10,

        ThisCardFitsRestriction = 100,
        TriggererFitsRestriction = 101,
        CoordsFitRestriction = 120,
        XFitsRestriction = 130,
        EffectSourceIsTriggerer = 149,

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
        FromDeck = 401,

        MaxPerTurn = 500,
        NotFromEffect = 501,
        MaxPerRound = 502
    }

    public TriggerRestrictions[] triggerRestrictions = new TriggerRestrictions[0];
    public BoardRestriction cardRestriction = new BoardRestriction(); //TODO refactor boardrestrictions to be part of cardrestriction
    public XRestriction xRestriction = new XRestriction();
    public SpaceRestriction spaceRestriction = new SpaceRestriction();
    public int maxTimesPerTurn = 1;
    public int maxPerRound = 1;

    public Card ThisCard { get; private set; }

    public ServerTrigger ThisTrigger { get; private set; }

    public void Initialize(ServerSubeffect subeff, Card thisCard, ServerTrigger thisTrigger)
    {
        Subeffect = subeff;
        cardRestriction.Subeffect = subeff;
        this.ThisCard = thisCard;
        this.ThisTrigger = thisTrigger;
    }

    public bool Evaluate(Card cardTriggerer, IStackable stackTrigger, Player triggerer, int? effX, (int x, int y)? space)
    {
        foreach(TriggerRestrictions r in triggerRestrictions)
        {
            switch (r)
            {
                case TriggerRestrictions.ThisCardTriggered:
                    if (cardTriggerer == ThisCard) continue;
                    else return false;
                case TriggerRestrictions.ThisCardInPlay:
                    if (ThisCard.Location == CardLocation.Field) continue;
                    else return false;
                case TriggerRestrictions.AugmentedCardTriggered:
                    if (ThisCard is AugmentCard aug && aug.AugmentedCard == cardTriggerer) continue;
                    else return false;
                case TriggerRestrictions.ThisCardFitsRestriction:
                    if (cardRestriction.Evaluate(ThisCard)) continue;
                    else return false;
                case TriggerRestrictions.TriggererFitsRestriction:
                    if (cardRestriction.Evaluate(cardTriggerer)) continue;
                    else return false;
                case TriggerRestrictions.CoordsFitRestriction:
                    if (space != null && spaceRestriction.Evaluate(space.Value)) continue;
                    else return false;
                case TriggerRestrictions.XFitsRestriction:
                    if (effX != null && xRestriction.Evaluate(effX.Value)) continue;
                    else return false;
                case TriggerRestrictions.EffectSourceIsTriggerer:
                    if (stackTrigger is Effect eff && eff.Source == cardTriggerer) continue;
                    else return false;
                //TODO make these into just something to do with triggered card fitting restriction
                case TriggerRestrictions.ControllerTriggered:
                    if (triggerer == ThisCard.Controller) continue;
                    else return false;
                case TriggerRestrictions.EnemyTriggered:
                    if (triggerer != ThisCard.Controller) continue;
                    else return false;
                case TriggerRestrictions.FriendlyTurn:
                    if (Subeffect.ServerGame.TurnPlayer == ThisCard.Controller) continue;
                    else return false;
                case TriggerRestrictions.EnemyTurn:
                    if (Subeffect.ServerGame.TurnPlayer != ThisCard.Controller) continue;
                    else return false;
                case TriggerRestrictions.FromField:
                    if (cardTriggerer.Location == CardLocation.Field) continue;
                    else return false;
                case TriggerRestrictions.FromDeck:
                    if (cardTriggerer.Location == CardLocation.Deck) continue;
                    else return false;
                case TriggerRestrictions.MaxPerTurn:
                    if (ThisTrigger.effToTrigger.TimesUsedThisTurn < maxTimesPerTurn) continue;
                    else return false;
                case TriggerRestrictions.NotFromEffect:
                    if (stackTrigger == null) continue;
                    else return false;
                case TriggerRestrictions.MaxPerRound:
                    if (ThisTrigger.effToTrigger.TimesUsedThisRound < maxPerRound) continue;
                    else return false;
                default:
                    Debug.LogError($"Unrecognized trigger restriction {r}");
                    break;
            }
        }

        return true;
    }
}
