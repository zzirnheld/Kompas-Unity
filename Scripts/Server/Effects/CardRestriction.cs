using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardRestriction : Restriction
{
    public enum CardRestrictions { CostsLessThanEqual, NameIs, SubtypesInclude, IsCharacter, IsSpell, IsAugment, Hand, Discard, Deck, Board} //to add later: N/E/S/W <=

    //because JsonUtility will fill in all values with defaults if not present
    public CardRestrictions[] restrictionsToCheck;

    public int costsLessThan;
    public string nameIs;
    public string[] subtypesInclude;


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
                case CardRestrictions.CostsLessThanEqual:
                    if (potentialTarget.GetCost() > costsLessThan) return false;
                    break;
                case CardRestrictions.NameIs:
                    if (potentialTarget.CardName != nameIs) return false;
                    break;
                case CardRestrictions.SubtypesInclude:
                    foreach(string s in subtypesInclude)
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
                default:
                    Debug.Log("You forgot to check for " + c);
                    return false;
            }
        }
        return true;
    }
}
