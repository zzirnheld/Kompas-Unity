using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActivationRestriction
{
    public Effect Effect { get; private set; }
    public Card Card => Effect.Source;

    public const string TimesPerTurn = "Max Times Per Turn";
    public const string TimesPerRound = "Max Times Per Round";
    public const string FriendlyTurn = "Friendly Turn";

    public int MaxTimes = 1;

    public string[] Restrictions = { };

    public void Initialize(Effect eff)
    {
        Effect = eff;
        Debug.Log($"Initializing activation restriction for {Card.CardName} with restrictions: {string.Join(", ", Restrictions)}");
    }

    public bool Evaluate(Player activator)
    {
        foreach(string r in Restrictions)
        {
            switch (r)
            {
                case TimesPerTurn:
                    if (Effect.TimesUsedThisTurn >= MaxTimes) return false;
                    break;
                case TimesPerRound:
                    if (Effect.TimesUsedThisRound >= MaxTimes) return false;
                    break;
                case FriendlyTurn:
                    if (Effect.Game.TurnPlayer != activator) return false;
                    break;
                default:
                    Debug.LogError($"You forgot to check for {r} in Activation Restriction switch");
                    return false;
            }
        }

        return true;
    }
}
