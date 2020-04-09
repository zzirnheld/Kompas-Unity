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
        SubtypesExclude = 6,
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
        WLTEC = 304,
        IndexInListGTEC = 500,
        IndexInListLTEC = 501
    } //to add later: N/E/S/W >=

    //because JsonUtility will fill in all values with defaults if not present
    public CardRestrictions[] restrictionsToCheck;

    public int costsLessThan;
    public string nameIs;
    public string[] subtypesInclude;
    public string[] subtypesExclude;
    public int constant;

    public virtual bool Evaluate (Card potentialTarget, int x)
    {
        if (potentialTarget == null) return false;

        foreach (CardRestrictions c in restrictionsToCheck)
        {
            Debug.Log("Considering restriction " + c + " when X equals " + x);
            switch (c)
            {
                case CardRestrictions.NameIs:
                    if (potentialTarget.CardName != nameIs) return false;
                    break;
                case CardRestrictions.SubtypesInclude:
                    foreach (string s in subtypesInclude)
                    {
                        if (potentialTarget.SubtypeText.IndexOf(s) == -1) return false;
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
                case CardRestrictions.SubtypesExclude:
                    foreach (string s in subtypesExclude)
                    {
                        if (potentialTarget.SubtypeText.IndexOf(s) != -1) return false;
                    }
                    break;
                case CardRestrictions.Hand:
                    if (potentialTarget.Location != CardLocation.Hand) return false;
                    break;
                case CardRestrictions.Deck:
                    if (potentialTarget.Location != CardLocation.Deck) return false;
                    break;
                case CardRestrictions.Discard:
                    if (potentialTarget.Location != CardLocation.Discard) return false;
                    break;
                case CardRestrictions.Board:
                    if (potentialTarget.Location != CardLocation.Field) return false;
                    break;
                case CardRestrictions.NLTEX:
                    if (!(potentialTarget is CharacterCard charC1)) return false;
                    if (charC1.N > x) return false;
                    break;
                case CardRestrictions.ELTEX:
                    if (!(potentialTarget is CharacterCard charC2)) return false;
                    if (charC2.E > x) return false;
                    break;
                case CardRestrictions.SLTEX:
                    if (!(potentialTarget is CharacterCard charC3)) return false;
                    if (charC3.S > x) return false;
                    break;
                case CardRestrictions.WLTEX:
                    if (!(potentialTarget is CharacterCard charC4)) return false;
                    if (charC4.N > x) return false;
                    break;
                case CardRestrictions.NLTEC:
                    if (!(potentialTarget is CharacterCard charC5)) return false;
                    if (charC5.N > constant) return false;
                    break;
                case CardRestrictions.ELTEC:
                    if (!(potentialTarget is CharacterCard charC6)) return false;
                    if (charC6.E > constant) return false;
                    break;
                case CardRestrictions.SLTEC:
                    if (!(potentialTarget is CharacterCard charC7)) return false;
                    if (charC7.S > constant) return false;
                    break;
                case CardRestrictions.WLTEC:
                    if (!(potentialTarget is CharacterCard charC8)) return false;
                    if (charC8.W > constant) return false;
                    break;
                case CardRestrictions.IndexInListGTEC:
                    if (potentialTarget.IndexInList < constant) return false;
                    break;
                case CardRestrictions.IndexInListLTEC:
                    if (potentialTarget.IndexInList > constant) return false;
                    break;
                default:
                    Debug.Log("You forgot to check for " + c);
                    return false;
            }
        }

        Debug.Log(potentialTarget.CardName + " fits the restriction");
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="potentialTarget"></param>
    /// <param name="actuallyTargetThis">Whether effect resolution should continue if this is a valid target</param>
    /// <returns></returns>
    public virtual bool Evaluate(Card potentialTarget)
    {
        return Evaluate(potentialTarget, subeffect.parent.X);
    }
}
