using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeXByTargetValueSubeffect : ServerSubeffect
{
    public const string Cost = "Cost";
    public const string S = "S";
    public const string W = "W";
    public const string C = "C";
    public const string DistanceToTarget = "Distance to Target";

    public string whatToCount;

    public int multiplier = 1;
    public int divisor = 1;
    public int modifier = 0;

    private int BaseCount
    {
        get
        {
            switch (whatToCount)
            {
                case Cost: return Target.Cost;
                case S: return Target.S;
                case W: return Target.W;
                case C: return Target.C;
                case DistanceToTarget:
                    return Source.DistanceTo(Target);
                default:
                    throw new System.ArgumentException($"Invalid 'what to count' string {whatToCount} in x by gamestate value subeffect");
            }
        }
    }

    protected int Count => (BaseCount * multiplier / divisor) + modifier;

    public override bool Resolve()
    {
        Effect.X += Count;
        return ServerEffect.ResolveNextSubeffect();
    }
}
