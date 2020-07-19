using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetXSubeffect : ServerSubeffect
{
    public int multiplier = 1;
    public int divisor = 1;
    public int modifier = 0;

    public int Count { get => Effect.X * multiplier / divisor + modifier; }

    public override bool Resolve()
    {
        Effect.X = Count;
        return ServerEffect.ResolveNextSubeffect();
    }
}
