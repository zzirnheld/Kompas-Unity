using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardRestriction : Restriction
{
    public enum CardRestrictions {
        NameIs = 1,
        SubtypesInclude = 2,
        IsCharacter = 3,
        IsSpell = 4,
        IsAugment = 5,
        Hand = 100,
        Discard = 101,
        Deck = 102,
        Board = 103,
        NLTEX = 200, //N <= X
        ELTEX = 201,
        SLTEX = 202,
        WLTEX = 203,
        NLTEC = 300, //N <= constant
        ELTEC = 302,
        SLTEC = 303,
        WLTEC = 304
    } //to add later: N/E/S/W <=

    //because JsonUtility will fill in all values with defaults if not present
    public CardRestrictions[] restrictionsToCheck;

    public int costsLessThan;
    public string nameIs;
    public string[] subtypesInclude;
    public int constant;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="potentialTarget"></param>
    /// <param name="actuallyTargetThis">Whether effect resolution should continue if this is a valid target</param>
    /// <returns></returns>
    public override bool Evaluate(Card potentialTarget)
    {
        if (potentialTarget == null) return false;

        foreach(CardRestrictions c in restrictionsToCheck)
        {
            Debug.Log("Considering restriction " + c);
            switch (c)
            {
                case CardRestrictions.NameIs:
                    if (potentialTarget.CardName != nameIs) return false;
                    break;
                case CardRestrictions.SubtypesInclude:
                    foreach (string s in subtypesInclude)
                    {
                        if (Array.IndexOf(potentialTarget.Subtypes, s) == -1) return false;
                    }
                    break;
                case CardRestrictions.IsCharacter:
                    if (!(potentialTarget is CharacterCard)) return false;
                    break;
                case CardRestrictions.IsSpell:
                    if (!(potentialTarget is SpellCard)) return false;
                    break;
                case CardRestrictions.IsAugment:
                    if (!(potentialTarget is AugmentCard)) return false;
                    break;
                case CardRestrictions.Hand:
                    if (potentialTarget.Location != Card.CardLocation.Hand) return false;
                    break;
                case CardRestrictions.Deck:
                    if (potentialTarget.Location != Card.CardLocation.Deck) return false;
                    break;
                case CardRestrictions.Discard:
                    if (potentialTarget.Location != Card.CardLocation.Discard) return false;
                    break;
                case CardRestrictions.Board:
                    if (potentialTarget.Location != Card.CardLocation.Field) return false;
                    break;
                case CardRestrictions.NLTEX:
                    if (!(potentialTarget is CharacterCard charC)) return false;
                    if (charC.N > subeffect.parent.X) return false; //TODO set x as command
                    break;
                case CardRestrictions.ELTEX:
                    break;
                case CardRestrictions.SLTEX:
                    break;
                case CardRestrictions.WLTEX:
                    break;
                case CardRestrictions.NLTEC:
                    break;
                case CardRestrictions.ELTEC:
                    break;
                case CardRestrictions.SLTEC:
                    break;
                case CardRestrictions.WLTEC:
                    break;
                default:
                    Debug.Log("You forgot to check for " + c);
                    return false;
            }
        }
        return true;
    }
}
