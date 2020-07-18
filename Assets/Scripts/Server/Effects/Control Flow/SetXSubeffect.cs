using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetXSubeffect : ServerSubeffect
{
    public int Multiplier = 1;
    public int Divisor = 1;
    public int Modifier = 0;

    public int Count { get => Effect.X * Multiplier / Divisor + Modifier; }

    public override bool Resolve()
    {
        Effect.X = Count;
        return ServerEffect.ResolveNextSubeffect();
    }
}
