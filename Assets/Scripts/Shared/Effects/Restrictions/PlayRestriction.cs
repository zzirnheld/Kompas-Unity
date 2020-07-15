using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayRestriction
{
    public GameCard Card { get; private set; }

    public const string PlayedByCardOwner = "Played By Card Owner";
    public const string FromHand = "From Hand";
    public const string StandardPlayRestriction = "Adjacent to Friendly Card";
    public const string OnFriendlyCard = "On Friendly Card";
    public const string FriendlyTurnIfNotFast = "Friendly Turn";
    public const string HasCostInPips = "Has Cost in Pips";
    public const string NothingIsResolving = "Nothing is Resolving";
    public const string NotNormally = "Cannot be Played Normally";
    public const string MustNormally = "Must be Played Normally";

    public string[] NormalRestrictions = { PlayedByCardOwner, FromHand, StandardPlayRestriction, FriendlyTurnIfNotFast, HasCostInPips, NothingIsResolving };
    public string[] EffectRestrictions = { };
    
    public void SetInfo(GameCard card)
    {
        Card = card;
    }

    public bool EvaluateNormalPlay(int x, int y, Player player)
    {
        foreach(string r in NormalRestrictions)
        {
            switch (r)
            {
                case PlayedByCardOwner:
                    if (player != Card.Owner) return false;
                    break;
                case FromHand:
                    if (Card.Location != CardLocation.Hand) return false;
                    break;
                case StandardPlayRestriction:
                    if (!Card.Game.ValidStandardPlaySpace(x, y, Card.Controller)) return false;
                    break;
                case OnFriendlyCard:
                    if (Card.Game.boardCtrl.GetCardAt(x, y)?.Controller != Card.Controller) return false;
                    break;
                case HasCostInPips:
                    if (Card.Controller.Pips < Card.Cost) return false;
                    break;
                case FriendlyTurnIfNotFast:
                    if (!Card.Fast && Card.Game.TurnPlayer != Card.Controller) return false;
                    break;
                case NothingIsResolving:
                    if (Card.Game.CurrStackEntry != null) return false;
                    break;
                case NotNormally:
                    return false;
                default:
                    Debug.LogError($"You forgot to check for condition {r} in Normal Play for PlayRestriction");
                    return false;
            }
        }

        return true;
    }

    public bool EvaluateEffectPlay(int x, int y, Effect effect)
    {
        foreach (string r in EffectRestrictions)
        {
            switch (r)
            {
                case StandardPlayRestriction:
                    if (!Card.Game.ValidStandardPlaySpace(x, y, Card.Controller)) return false;
                    break;
                case FriendlyTurnIfNotFast:
                    if (!Card.Fast && Card.Game.TurnPlayer != Card.Controller) return false;
                    break;
                case MustNormally:
                    return false;
                default:
                    Debug.LogError($"You forgot to check for condition {r} in Effect Play for PlayRestriction");
                    return false;
            }
        }

        return true;
    }
}
