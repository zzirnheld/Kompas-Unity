using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeXByTargetValueSubeffect : ServerSubeffect
{
    public const string Cost = "Cost";
    public const string W = "W";

    public string WhatToCount;

    public int Multiplier = 1;
    public int Divisor = 1;
    public int Modifier = 0;

    private int BaseCount
    {
        get
        {
            switch (WhatToCount)
            {
                case Cost:
                    return Target.Cost;
                case W:
                    if (Target is CharacterCard charTarget) return charTarget.W;
                    else throw new System.ArgumentException($"Asked for W of non-character target");
                default:
                    throw new System.ArgumentException($"Invalid 'what to count' string {WhatToCount} in x by gamestate value subeffect");
            }
        }
    }

    protected int Count => (BaseCount * Multiplier / Divisor) + Modifier;

    public override void Resolve()
    {
        Effect.X += Count;
        ServerEffect.ResolveNextSubeffect();
    }
}
