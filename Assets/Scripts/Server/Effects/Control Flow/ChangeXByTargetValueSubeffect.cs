using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeXByTargetValueSubeffect : ServerSubeffect
{
    public const string Cost = "Cost";

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
