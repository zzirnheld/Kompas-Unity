using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardRestriction
{
    public Subeffect Subeffect { get; set; }

    public enum CardRestrictions {
        NameIs = 1,
        SubtypesInclude = 2,
        IsCharacter = 3,
        IsSpell = 4,
        IsAugment = 5,
        SubtypesExclude = 6,
        Friendly = 7,
        SameOwner = 8,
        Enemy = 9,
        //location
        Hand = 100,
        Discard = 101,
        Deck = 102,
        Board = 103,
        //stats
        NLTEX = 200, //N <= X
        ELTEX = 201,
        SLTEX = 202,
        WLTEX = 203,
        CostLTEX = 204,
        NEX = 210, //N == X
        EEX = 211,
        SEX = 212,
        WEX = 213,
        NLTEC = 300, //N <= constant
        ELTEC = 302,
        SLTEC = 303,
        WLTEC = 304,
        //index in list
        IndexInListGTEC = 500,
        IndexInListLTEC = 501,
        //misc
        NotAlreadyTarget = 600
    } //to add later: N/E/S/W >=

    //because JsonUtility will fill in all values with defaults if not present
    public CardRestrictions[] restrictionsToCheck = new CardRestrictions[0];

    public int costsLessThan;
    public string nameIs;
    public string[] subtypesInclude;
    public string[] subtypesExclude;
    public int constant;

    public virtual bool Evaluate (Card potentialTarget, int x)
    {
        if (potentialTarget == null) return false;

        //some restrictions require checking if the target is a character. no reason to check twice
        CharacterCard charCard = potentialTarget as CharacterCard;

        foreach (CardRestrictions c in restrictionsToCheck)
        {
            //Debug.Log("Considering restriction " + c + " when X equals " + x);
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
                case CardRestrictions.Friendly:
                    if (potentialTarget.Controller != Subeffect.Controller) return false;
                    break;
                case CardRestrictions.Enemy:
                    if (potentialTarget.Controller == Subeffect.Controller) return false;
                    break;
                case CardRestrictions.SameOwner:
                    if (potentialTarget.Owner != Subeffect.Controller) return false;
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
                    if (charCard == null) return false;
                    if (charCard.N > x) return false;
                    break;
                case CardRestrictions.ELTEX:
                    if (charCard == null) return false;
                    if (charCard.E > x) return false;
                    break;
                case CardRestrictions.SLTEX:
                    if (charCard == null) return false;
                    if (charCard.S > x) return false;
                    break;
                case CardRestrictions.WLTEX:
                    if (charCard == null) return false;
                    if (charCard.W > x) return false;
                    break;
                case CardRestrictions.CostLTEX:
                    if (potentialTarget.Cost > x) return false;
                    break;
                case CardRestrictions.NEX:
                    if (charCard == null) return false;
                    if (charCard.N != x) return false;
                    break;
                case CardRestrictions.EEX:
                    if (charCard == null) return false;
                    if (charCard.E != x) return false;
                    break;
                case CardRestrictions.SEX:
                    if (charCard == null) return false;
                    if (charCard.S != x) return false;
                    break;
                case CardRestrictions.WEX:
                    if (charCard == null) return false;
                    if (charCard.W != x) return false;
                    break;
                case CardRestrictions.NLTEC:
                    if (charCard == null) return false;
                    if (charCard.N > constant) return false;
                    break;
                case CardRestrictions.ELTEC:
                    if (charCard == null) return false;
                    if (charCard.E > constant) return false;
                    break;
                case CardRestrictions.SLTEC:
                    if (charCard == null) return false;
                    if (charCard.S > constant) return false;
                    break;
                case CardRestrictions.WLTEC:
                    if (charCard == null) return false;
                    if (charCard.W > constant) return false;
                    break;
                case CardRestrictions.IndexInListGTEC:
                    if (potentialTarget.IndexInList < constant) return false;
                    break;
                case CardRestrictions.IndexInListLTEC:
                    if (potentialTarget.IndexInList > constant) return false;
                    break;
                case CardRestrictions.NotAlreadyTarget:
                    if (Subeffect.Effect.targets.Contains(potentialTarget)) return false;
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
        return Evaluate(potentialTarget, Subeffect.Effect.X);
    }
}
