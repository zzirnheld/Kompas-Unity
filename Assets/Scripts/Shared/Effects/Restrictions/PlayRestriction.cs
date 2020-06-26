using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRestriction
{
    public GameCard Card { get; private set; }

    public const string PlayedByCardOwner = "Played By Card Owner";
    public const string FromHand = "From Hand";
    public const string AdjacentToFriendlyCard = "Adjacent to Friendly Card";
    public const string OnFriendlyCard = "On Friendly Card";
    public const string FriendlyTurn = "Friendly Turn";
    public const string HasCostInPips = "Has Cost in Pips";
    public const string NotNormally = "Cannot be Played Normally";
    public const string MustNormally = "Must be Played Normally";

    public string[] NormalRestrictions = { PlayedByCardOwner, FromHand, AdjacentToFriendlyCard, FriendlyTurn, HasCostInPips };
    public string[] EffectRestrictions = { AdjacentToFriendlyCard };

    private int x;
    private int y;
    
    public void SetInfo(GameCard card)
    {
        Card = card;
    }

    private bool CardIsAdjToCoordsAndFriendly(GameCard c)
    {
        return c != null && c.IsAdjacentTo(x, y) && c.Controller == Card.Controller;
    }

    public bool EvaluateNormalPlay(int x, int y, Player player)
    {
        this.x = x;
        this.y = y;

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
                    if (!Card.Game.boardCtrl.ExistsCardOnBoard(c => CardIsAdjToCoordsAndFriendly(c))) return false;
                    break;
                case OnFriendlyCard:
                    if (Card.Game.boardCtrl.GetCardAt(x, y)?.Controller != Card.Controller) return false;
                    break;
                case HasCostInPips:
                    if (Card.Controller.Pips < Card.Cost) return false;
                    break;
                case FriendlyTurn:
                    if (Card.Game.TurnPlayer != Card.Controller) return false;
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
        this.x = x;
        this.y = y;

        foreach (string r in EffectRestrictions)
        {
            switch (r)
            {
                case AdjacentToFriendlyCard:
                    if (!Card.Game.boardCtrl.ExistsCardOnBoard(c => CardIsAdjToCoordsAndFriendly(c))) return false;
                    break;
                case FriendlyTurn:
                    if (Card.Game.TurnPlayer != Card.Controller) return false;
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
