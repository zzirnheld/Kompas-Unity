using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActivationRestriction
{
    public Effect Effect { get; private set; }
    public GameCard Card => Effect.Source;

    public const string TimesPerTurn = "Max Times Per Turn";
    public const string TimesPerRound = "Max Times Per Round";
    public const string FriendlyTurn = "Friendly Turn";
    public const string EnemyTurn = "Enemy Turn";
    public const string InPlay = "In Play";

    public int maxTimes = 1;

    public string[] activationRestrictions = { };

    public void Initialize(Effect eff)
    {
        Effect = eff;
        Debug.Log($"Initializing activation restriction for {Card.CardName} with restrictions: {string.Join(", ", activationRestrictions)}");
    }

    public bool Evaluate(Player activator)
    {
        foreach(string r in activationRestrictions)
        {
            switch (r)
            {
                case TimesPerTurn:
                    if (Effect.TimesUsedThisTurn >= maxTimes) return false;
                    break;
                case TimesPerRound:
                    if (Effect.TimesUsedThisRound >= maxTimes) return false;
                    break;
                case FriendlyTurn:
                    if (Effect.Game.TurnPlayer != activator) return false;
                    break;
                case EnemyTurn:
                    if (Effect.Game.TurnPlayer != activator) return false;
                    break;
                case InPlay:
                    if (Effect.Source.Location != CardLocation.Field) return false;
                    break;
                default:
                    Debug.LogError($"You forgot to check for {r} in Activation Restriction switch");
                    return false;
            }
        }

        return true;
    }
}
