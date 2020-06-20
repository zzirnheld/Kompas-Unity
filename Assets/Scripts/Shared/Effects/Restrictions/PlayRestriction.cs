using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRestriction
{
    public Card Card { get; private set; }

    public const string PlayedByCardOwner = "Played By Card Owner";
    public const string FromHand = "From Hand";
    public const string AdjacentToFriendlyCard = "Adjacent to Friendly Card";
    public const string FriendlyTurn = "Friendly Turn";
    public const string NotNormally = "Cannot be Played Normally";
    public const string MustNormally = "Must be Played Normally";

    public string[] NormalRestrictions = { PlayedByCardOwner, FromHand, AdjacentToFriendlyCard, FriendlyTurn };
    public string[] EffectRestrictions = { AdjacentToFriendlyCard };

    public void SetInfo(Card card)
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
                case AdjacentToFriendlyCard:
                    if (!Card.game.boardCtrl.ExistsCardOnBoard(c => c != null && c.IsAdjacentTo(x, y) && c.Controller == Card.Controller)) return false;
                    break;
                case FriendlyTurn:
                    if (Card.game.TurnPlayer != Card.Controller) return false;
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
                case AdjacentToFriendlyCard:
                    if (!Card.IsAdjacentTo(x, y)) return false;
                    break;
                case FriendlyTurn:
                    if (Card.game.TurnPlayer != Card.Controller) return false;
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
