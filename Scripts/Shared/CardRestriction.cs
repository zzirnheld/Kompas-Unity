using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardRestriction : Restriction
{
    public enum CardRestrictions { CostsLessThanEqual, NameIs, SubtypesInclude}
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
    public virtual bool Evaluate(Card potentialTarget, bool actuallyTargetThis)
    {
        if (potentialTarget == null) return false;

        foreach(CardRestrictions c in restrictionsToCheck)
        {
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
                default:
                    Debug.Log("You forgot to check for " + c);
                    return false;
            }
        }

        if (actuallyTargetThis)
        {
            subeffect.parent.targets.Add(potentialTarget);
            subeffect.parent.ResolveSubeffect(subeffect.parent.effectIndex + 1);
        }

        return true;
    }
}
