using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CardRestriction
{
    public Subeffect Subeffect { get; set; }

    //TODO add "can summon" restriction that checks that there exists
    //a space that the card can be played to
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
        Summoned = 10,
        DistinctName = 11,
        SameName = 12,
        Avatar = 50,
        //location
        Hand = 100,
        Discard = 101,
        Deck = 102,
        Board = 103,
        LocationInList = 104,
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
        CostEX = 214,
        NLTX = 220,
        ELTX = 221,
        SLTX = 222,
        WLTX = 223,
        NLTEC = 300, //N <= constant
        ELTEC = 302,
        SLTEC = 303,
        WLTEC = 304,
        //index in list
        IndexInListGTEC = 500,
        IndexInListLTEC = 501,
        IndexInListLTEX = 551,
        //misc
        NotAlreadyTarget = 600,
        CanBePlayed = 601,
        EffectControllerCanPayCost = 602
    } //to add later: N/E/S/W >=

    //because JsonUtility will fill in all values with defaults if not present
    public CardRestrictions[] restrictionsToCheck = new CardRestrictions[0];

    public int costsLessThan;
    public string nameIs;
    public string[] subtypesInclude;
    public string[] subtypesExclude;
    public int constant;
    public CardLocation[] locations;
    public int costMultiplier = 1;
    public int costDivisor = 1;

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
                case CardRestrictions.Summoned:
                    if (!potentialTarget.Summoned) return false;
                    break;
                case CardRestrictions.DistinctName:
                    //checks if any target shares a name with this one
                    if (Subeffect.Effect.Targets.Where(card => card.CardName == potentialTarget.CardName).Any()) return false;
                    break;
                case CardRestrictions.SameName: //note: same name as this subeffect's target
                    if (Subeffect.Target.CardName != potentialTarget.CardName) return false;
                    break;
                case CardRestrictions.Avatar:
                    if (!(potentialTarget is AvatarCard)) return false;
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
                case CardRestrictions.LocationInList:
                    if (!locations.Contains(potentialTarget.Location)) return false;
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
                case CardRestrictions.CostEX:
                    if (potentialTarget.Cost != x) return false;
                    break;
                case (CardRestrictions.NLTX):
                    if (charCard == null) return false;
                    if (charCard.N >= x) return false;
                    break;
                case (CardRestrictions.ELTX):
                    if (charCard == null) return false;
                    if (charCard.E >= x) return false;
                    break;
                case (CardRestrictions.SLTX):
                    if (charCard == null) return false;
                    if (charCard.S >= x) return false;
                    break;
                case (CardRestrictions.WLTX):
                    if (charCard == null) return false;
                    if (charCard.W >= x) return false;
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
                case CardRestrictions.IndexInListLTEX:
                    if (potentialTarget.IndexInList > Subeffect.Effect.X) return false;
                    break;
                case CardRestrictions.NotAlreadyTarget:
                    if (Subeffect.Effect.Targets.Contains(potentialTarget)) return false;
                    break;
                case CardRestrictions.CanBePlayed:
                    bool found = false;
                    for(int i = 0; i < 7 && !found; i++)
                    {
                        for(int j = 0; j < 7 && !found; j++)
                        {
                            if (potentialTarget.PlayRestriction.EvaluateEffectPlay(i, j, Subeffect.Effect))
                            {
                                found = true;
                            }
                        }
                    }
                    if (found) break;
                    else return false;
                case CardRestrictions.EffectControllerCanPayCost:
                    if (Subeffect.Effect.Controller.pips < potentialTarget.Cost * costMultiplier / costDivisor) return false;
                    break;
                default:
                    Debug.LogError($"You forgot to implement a check for {c}");
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
