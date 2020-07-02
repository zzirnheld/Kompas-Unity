using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackRestriction
{
    public const string ThisIsCharacter = "This is Character";
    public const string DefenderIsCharacter = "Defender is Character";
    public const string DefenderIsAdjacent = "Defender is Adjacent";
    public const string DefenderIsEnemy = "Defender is Enemy";

    public const string ThisIsActive = "This is Activated";

    public string[] Restrictions = new string[] { ThisIsCharacter, DefenderIsCharacter, DefenderIsAdjacent, DefenderIsEnemy };

    public GameCard Card { get; private set; }

    public void SetInfo(GameCard card)
    {
        Card = card;
    }

    public bool Evaluate(GameCard defender)
    {
        if (defender == null) return false;

        foreach(string r in Restrictions)
        {
            switch (r)
            {
                case ThisIsCharacter:
                    if (Card.CardType != 'C') return false;
                    break;
                case DefenderIsCharacter:
                    if (defender.CardType != 'C') return false;
                    break;
                case DefenderIsAdjacent:
                    if (!Card.IsAdjacentTo(defender)) return false;
                    break;
                case DefenderIsEnemy:
                    if (Card.Controller == defender.Controller) return false;
                    break;
                case ThisIsActive:
                    if (!Card.Activated) return false;
                    break;
                default:
                    throw new System.ArgumentException($"Could not understand attack restriction {r}");
            }
        }

        return true;
    }
}
