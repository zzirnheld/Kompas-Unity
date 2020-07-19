using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class XRestriction
{
    public const string LessThanEqualThisCost = "<=ThisCost";
    public const string LessThanEqualThisE = "<=ThisE";
    public const string Positive = ">0";
    public const string Negative = "<0";

    public string[] xRestrictions = new string[0];

    public Subeffect Subeffect { get; private set; }

    public void Initialize(Subeffect subeff)
    {
        Subeffect = subeff;
    }

    public bool Evaluate(int x)
    {
        foreach(var r in xRestrictions)
        {
            switch (r)
            {
                case Positive:
                    if (x <= 0) return false;
                    break;
                case Negative:
                    if (x >= 0) return false;
                    break;
                case LessThanEqualThisCost:
                    if (x > Subeffect.Source.Cost) return false;
                    break;
                case LessThanEqualThisE:
                    if (x > Subeffect.Source.E) return false;
                    break;
                default:
                    throw new System.ArgumentException($"Invalid X restriction {r} in X Restriction.");
            }
        }

        return true;
    }
}
