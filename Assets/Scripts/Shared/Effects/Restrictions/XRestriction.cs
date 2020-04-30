using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class XRestriction
{
    public const string LessThanEqualThisCost = "<=ThisCost";
    public const string Positive = ">0";

    public string[] Restrictions = new string[0];

    public Subeffect Subeffect;

    public bool Evaluate(int x)
    {
        foreach(string r in Restrictions)
        {
            switch (r)
            {
                case Positive:
                    if (x <= 0) return false;
                    break;
                case LessThanEqualThisCost:
                    if (Subeffect.Source.Cost > x) return false;
                    break;
                default:
                    throw new System.ArgumentException($"Invalid X restriction {r} in X Restriction.");
            }
        }

        return true;
    }
}
