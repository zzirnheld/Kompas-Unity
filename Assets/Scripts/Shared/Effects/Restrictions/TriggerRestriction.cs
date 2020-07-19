using KompasCore.Cards;
using KompasServer.Effects;
using UnityEngine;

namespace KompasCore.Effects
{
    [System.Serializable]
    public class TriggerRestriction
    {
        public Subeffect Subeffect { get; private set; }

        public const string ThisCardTriggered = "This Card Triggered"; //0,

        //todo deprecate this card in play to this card fits restriction
        public const string ThisCardInPlay = "This Card in Play"; //1,
        public const string AugmentedCardTriggered = "Augmented Card Triggered"; //10,

        public const string ThisCardFitsRestriction = "This Card Fits Restriction"; //100,
        public const string TriggererFitsRestriction = "Triggerer Fits Restriction"; //101,
        public const string CoordsFitRestriction = "Coords Fit Restriction"; //120,
        public const string XFitsRestriction = "X Fits Restriction"; //130,
        public const string EffectSourceIsThisCard = "Stackable Source is This Card"; //140,
        public const string EffectSourceIsTriggerer = "Stackable Source is Triggerer"; //149,

        public const string ControllerTriggered = "Controller Triggered"; //200,
        public const string EnemyTriggered = "Enemy Triggered"; //201,

        //note: turns are the exception to the rule in that they are the only triggers where the triggering event
        //(in their case, the turn passing)
        //happens before the trigger is called, 
        //instead of the trigger being called at the moment before it happens.
        //in short, note that the turn will pass, then the trigger for turn start is called.
        //this means that checking for friendly/enemy turn will check whose turn the current (just-changed-to) turn is.
        public const string FriendlyTurn = "Friendly Turn"; //300,
        public const string EnemyTurn = "Enemy Turn"; //301,

        public const string FromField = "From Field"; //400,
        public const string FromDeck = "From Deck"; //401,

        public const string MaxPerTurn = "Max Per Turn"; //500,
        public const string NotFromEffect = "Not From Effect"; //501,
        public const string MaxPerRound = "Max Per Round"; //502

        public string[] triggerRestrictions = new string[0];
        public CardRestriction cardRestriction = new CardRestriction(); //TODO refactor boardrestrictions to be part of cardrestriction
        public XRestriction xRestriction = new XRestriction();
        public SpaceRestriction spaceRestriction = new SpaceRestriction();
        public int maxTimesPerTurn = 1;
        public int maxPerRound = 1;

        public GameCard ThisCard { get; private set; }

        public ServerTrigger ThisTrigger { get; private set; }

        public void Initialize(ServerSubeffect subeff, GameCard thisCard, ServerTrigger thisTrigger)
        {
            Subeffect = subeff;
            cardRestriction.Initialize(subeff);
            xRestriction.Initialize(subeff);
            spaceRestriction.Initialize(subeff);
            this.ThisCard = thisCard;
            this.ThisTrigger = thisTrigger;
        }

        public bool Evaluate(ActivationContext context)
        {
            foreach (var r in triggerRestrictions)
            {
                switch (r)
                {
                    case ThisCardTriggered:
                        if (context.Card == ThisCard) continue;
                        else return false;
                    case ThisCardInPlay:
                        if (ThisCard.Location == CardLocation.Field) continue;
                        else return false;
                    case AugmentedCardTriggered:
                        if (context.Card == ThisCard.AugmentedCard) continue;
                        else return false;
                    case ThisCardFitsRestriction:
                        if (cardRestriction.Evaluate(ThisCard)) continue;
                        else return false;
                    case TriggererFitsRestriction:
                        if (cardRestriction.Evaluate(context.Card)) continue;
                        else return false;
                    case CoordsFitRestriction:
                        if (context.Space != null && spaceRestriction.Evaluate(context.Space.Value)) continue;
                        else return false;
                    case XFitsRestriction:
                        if (context.X != null && xRestriction.Evaluate(context.X.Value)) continue;
                        else return false;
                    case EffectSourceIsTriggerer:
                        if (context.Stackable is Effect eff && eff.Source == context.Card) continue;
                        else return false;
                    //TODO make these into just something to do with triggered card fitting restriction
                    case ControllerTriggered:
                        if (context.Triggerer == ThisCard.Controller) continue;
                        else return false;
                    case EnemyTriggered:
                        if (context.Triggerer != ThisCard.Controller) continue;
                        else return false;
                    case FriendlyTurn:
                        if (Subeffect.Game.TurnPlayer == ThisCard.Controller) continue;
                        else return false;
                    case EnemyTurn:
                        if (Subeffect.Game.TurnPlayer != ThisCard.Controller) continue;
                        else return false;
                    case FromField:
                        if (context.Card.Location == CardLocation.Field) continue;
                        else return false;
                    case FromDeck:
                        if (context.Card.Location == CardLocation.Deck) continue;
                        else return false;
                    case MaxPerTurn:
                        if (ThisTrigger.effToTrigger.TimesUsedThisTurn < maxTimesPerTurn) continue;
                        else return false;
                    case NotFromEffect:
                        if (context.Stackable is Effect) return false;
                        break;
                    case MaxPerRound:
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
}